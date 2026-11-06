using System;
using System.Collections.Generic;

namespace SimulatorCisteniVzduchu.Systems;

/// <summary>
/// Celý stav jedné rozehrané hry: kolik má hráč smogu, co si koupil
/// a jak daleko je k čistému vzduchu. Nic nekreslí – jen počítá.
/// </summary>
public class GameState
{
    public Balance Balance { get; }

    /// <summary>Smog, který má hráč k utracení (herní měna).</summary>
    public double Smog { get; private set; }

    /// <summary>Kolik smogu hráč za celou hru vyčistil (utrácení to nesnižuje).</summary>
    public double TotalCleaned { get; private set; }

    private readonly Dictionary<string, int> _owned = new();

    public GameState(Balance balance)
    {
        Balance = balance;
    }

    public double PerClick
    {
        get
        {
            double value = 1; // holá ruka
            foreach (var upgrade in Balance.Upgrades)
                value += upgrade.PerClick * Owned(upgrade);
            return value;
        }
    }

    public double PerSecond
    {
        get
        {
            double value = 0;
            foreach (var upgrade in Balance.Upgrades)
                value += upgrade.PerSecond * Owned(upgrade);
            return value;
        }
    }

    /// <summary>Jak daleko je hráč k vítězství, 0..1.</summary>
    public double Progress => Math.Clamp(TotalCleaned / Balance.SmogToWin, 0, 1);

    /// <summary>Procento kyslíku v atmosféře (od startOxygen do 100).</summary>
    public double Oxygen => Balance.StartOxygen + (100 - Balance.StartOxygen) * Progress;

    public bool IsWon => Progress >= 1;

    public int Owned(UpgradeDefinition upgrade) =>
        _owned.TryGetValue(upgrade.Id, out int count) ? count : 0;

    public double CostOf(UpgradeDefinition upgrade) =>
        Math.Ceiling(upgrade.BaseCost * Math.Pow(Balance.CostGrowth, Owned(upgrade)));

    public bool CanAfford(UpgradeDefinition upgrade) => Smog >= CostOf(upgrade);

    /// <summary>
    /// Vylepšení se v obchodě ukáže, až se k němu hráč přiblíží – ať je co objevovat.
    /// </summary>
    public bool IsRevealed(UpgradeDefinition upgrade, int index) =>
        index < 2 || Owned(upgrade) > 0 || TotalCleaned >= upgrade.BaseCost * 0.5;

    /// <summary>Hráč klikl na kytku. Vrací, kolik smogu získal.</summary>
    public double Click()
    {
        double gained = PerClick;
        AddSmog(gained);
        return gained;
    }

    public void Update(double deltaSeconds)
    {
        AddSmog(PerSecond * deltaSeconds);
    }

    public bool TryBuy(UpgradeDefinition upgrade)
    {
        double cost = CostOf(upgrade);
        if (Smog < cost)
            return false;

        Smog -= cost;
        _owned[upgrade.Id] = Owned(upgrade) + 1;
        return true;
    }

    public void AddSmog(double amount)
    {
        Smog += amount;
        TotalCleaned += amount;
    }

    // ---------- Ukládání ----------

    public SaveData ToSave() => new()
    {
        Smog = Smog,
        TotalCleaned = TotalCleaned,
        Owned = new Dictionary<string, int>(_owned),
    };

    public void LoadFrom(SaveData save)
    {
        Smog = save.Smog;
        TotalCleaned = save.TotalCleaned;
        _owned.Clear();
        foreach (var (id, count) in save.Owned)
            _owned[id] = count;
    }
}
