using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SimulatorCisteniVzduchu.Systems;

/// <summary>Jedna továrna záporáka stojící v krajině.</summary>
public class Factory
{
    public int Slot { get; init; }
    public int Health { get; set; }
    public int MaxHealth { get; init; }

    /// <summary>Pro animaci zatřesení po zásahu (1 = právě zasažena, klesá k 0).</summary>
    public float HitShake { get; set; }

    /// <summary>Pro animaci vyrůstání ze země (0 = právě postavena, 1 = hotová).</summary>
    public float BuildProgress { get; set; } = 1;

    /// <summary>Kde továrna stojí a kam se na ni kliká (plátno 1920×1080).</summary>
    public Rectangle Bounds => Villain.FactoryBounds(Slot);
}

/// <summary>
/// Záporák SMOGOTRON 3000. Hlídá příběhové milníky, staví továrny
/// a ty znečišťují vzduch (snižují kyslík). Nic nekreslí – jen počítá
/// a dává scéně vědět přes události (OnMilestone, OnTaunt...).
/// </summary>
public class Villain
{
    /// <summary>X-ové pozice míst, kam lze postavit továrnu (mimo kytku a obchod).</summary>
    private static readonly int[] SlotX = { 200, 1160, 400, 960 };
    private const int FactoryWidth = 170;
    private const int FactoryHeight = 240;
    private const int GroundTop = 820;

    private readonly GameState _state;
    private readonly Story _story;
    private readonly FactoryBalance _balance;
    private readonly Random _random = new();
    private readonly List<Factory> _factories = new();
    private readonly HashSet<double> _seenMilestones = new();
    private double _spawnTimer;

    public IReadOnlyList<Factory> Factories => _factories;
    public bool FactoriesActive { get; private set; }
    public int FactoriesBuilt { get; private set; }
    public int FactoriesDestroyed { get; private set; }

    /// <summary>Spustí se, když hráč dosáhne příběhového milníku (scéna ukáže dialog).</summary>
    public event Action<StoryMilestone> OnMilestone;
    /// <summary>Krátká hláška záporáka (scéna ukáže bublinu).</summary>
    public event Action<string> OnTaunt;
    public event Action<Factory> OnFactoryBuilt;
    /// <summary>Továrna zničena; druhý parametr je odměna ve smogu.</summary>
    public event Action<Factory, double> OnFactoryDestroyed;

    public Villain(GameState state, Story story)
    {
        _state = state;
        _story = story;
        _balance = state.Balance.Factories;
    }

    public static Rectangle FactoryBounds(int slot) => new(
        SlotX[slot] - FactoryWidth / 2,
        GroundTop + 10 - FactoryHeight,
        FactoryWidth,
        FactoryHeight);

    /// <summary>Kolik smogu za sekundu vracejí všechny továrny zpátky do vzduchu.</summary>
    public double PollutionPerSecond =>
        _factories.Count(f => f.BuildProgress >= 1) * PollutionPerFactory;

    /// <summary>
    /// Kolik smogu za sekundu vrací jedna továrna. Aspoň tolik, aby kyslík znatelně klesal
    /// (minOxygenDrain), a s rostoucím čištěním hráče roste i ona (pollutionFraction).
    /// </summary>
    public double PollutionPerFactory =>
        Math.Max(_state.OxygenToSmog(_balance.MinOxygenDrain), _state.PerSecond * _balance.PollutionFraction);

    private double SpawnInterval =>
        MathHelper.Lerp((float)_balance.SpawnIntervalStart, (float)_balance.SpawnIntervalEnd, (float)_state.Progress);

    public void Update(double deltaSeconds)
    {
        CheckMilestones();

        foreach (var factory in _factories)
        {
            factory.HitShake = Math.Max(0, factory.HitShake - (float)deltaSeconds * 5);
            factory.BuildProgress = Math.Min(1, factory.BuildProgress + (float)deltaSeconds * 1.5f);
        }

        if (!FactoriesActive)
            return;

        _spawnTimer += deltaSeconds;
        if (_factories.Count >= _balance.MaxCount)
        {
            // Všechna místa jsou plná – po zničení továrny chvíli počkáme, než přijde další.
            _spawnTimer = Math.Min(_spawnTimer, SpawnInterval * 0.5);
        }
        else if (_spawnTimer >= SpawnInterval)
        {
            _spawnTimer = 0;
            SpawnFactory();
        }

        _state.AddPollution(PollutionPerSecond * deltaSeconds);
    }

    private void CheckMilestones()
    {
        foreach (var milestone in _story.Milestones.OrderBy(m => m.Oxygen))
        {
            if (_seenMilestones.Contains(milestone.Oxygen) || _state.Oxygen < milestone.Oxygen)
                continue;

            _seenMilestones.Add(milestone.Oxygen);
            if (milestone.StartsFactories && !FactoriesActive)
            {
                FactoriesActive = true;
                // První továrna přijde brzo po dialogu, ať hráč hned vidí, o co jde.
                _spawnTimer = Math.Max(0, SpawnInterval - 4);
            }
            OnMilestone?.Invoke(milestone);
            return; // jeden dialog najednou
        }
    }

    public void SpawnFactory()
    {
        var freeSlots = Enumerable.Range(0, SlotX.Length)
            .Where(slot => _factories.All(f => f.Slot != slot))
            .ToList();
        if (freeSlots.Count == 0)
            return;

        int health = Math.Min(_balance.MaxHealth, _balance.BaseHealth + _balance.HealthGrowth * FactoriesBuilt);
        var factory = new Factory
        {
            Slot = freeSlots[_random.Next(freeSlots.Count)],
            Health = health,
            MaxHealth = health,
            BuildProgress = 0,
        };
        _factories.Add(factory);
        FactoriesBuilt++;

        OnFactoryBuilt?.Invoke(factory);
        if (_story.FactoryBuiltTaunts.Count > 0)
            OnTaunt?.Invoke(_story.FactoryBuiltTaunts[_random.Next(_story.FactoryBuiltTaunts.Count)]);
    }

    /// <summary>
    /// Hráč klikl na místo <paramref name="point"/>. Pokud tam stojí továrna, ubere jí život.
    /// Vrací zasaženou továrnu, nebo null.
    /// </summary>
    public Factory TryHit(Point point)
    {
        var factory = _factories.LastOrDefault(f => f.Bounds.Contains(point));
        if (factory == null)
            return null;

        factory.Health--;
        factory.HitShake = 1;

        if (factory.Health <= 0)
        {
            _factories.Remove(factory);
            FactoriesDestroyed++;
            double bounty = Math.Max(25, Math.Max(_state.PerSecond, PollutionPerFactory) * _balance.BountySeconds);
            _state.AddSmog(bounty);

            OnFactoryDestroyed?.Invoke(factory, bounty);
            if (_story.FactoryDestroyedTaunts.Count > 0)
                OnTaunt?.Invoke(_story.FactoryDestroyedTaunts[_random.Next(_story.FactoryDestroyedTaunts.Count)]);
        }

        return factory;
    }

    // ---------- Ukládání ----------

    public VillainSave ToSave() => new()
    {
        SeenMilestones = _seenMilestones.ToList(),
        FactoriesActive = FactoriesActive,
        FactoriesBuilt = FactoriesBuilt,
        FactoriesDestroyed = FactoriesDestroyed,
        SpawnTimer = _spawnTimer,
        Factories = _factories
            .Select(f => new FactorySave { Slot = f.Slot, Health = f.Health, MaxHealth = f.MaxHealth })
            .ToList(),
    };

    public void LoadFrom(VillainSave save)
    {
        if (save == null)
            return;

        _seenMilestones.Clear();
        foreach (double oxygen in save.SeenMilestones)
            _seenMilestones.Add(oxygen);

        FactoriesActive = save.FactoriesActive;
        FactoriesBuilt = save.FactoriesBuilt;
        FactoriesDestroyed = save.FactoriesDestroyed;
        _spawnTimer = save.SpawnTimer;

        _factories.Clear();
        foreach (var f in save.Factories)
        {
            if (f.Slot < 0 || f.Slot >= SlotX.Length || _factories.Any(x => x.Slot == f.Slot))
                continue;
            _factories.Add(new Factory { Slot = f.Slot, Health = f.Health, MaxHealth = f.MaxHealth });
        }
    }
}
