using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace SimulatorCisteniVzduchu.Systems;

/// <summary>
/// Přehrávání zvuků podle jména, např. Sounds.Play("Sounds/villain_laugh").
///
/// Zvuky se načítají přímo z WAV souborů ve složce Content (Content/Sounds/villain_laugh.wav),
/// v MGCB Editoru se nic přidávat nemusí. Varianty se střídají samy:
/// když existují click_flower_01.wav, click_flower_02.wav..., Play("Sounds/click_flower")
/// vybere náhodně jednu z nich.
///
/// Když zvuk ještě není nahraný, nic se nestane – hra funguje i bez zvuků.
/// </summary>
public static class Sounds
{
    private const int MaxVariants = 9;

    private static ContentManager _content;
    private static string _root;
    private static readonly Dictionary<string, List<SoundEffect>> Cache = new();
    private static readonly Random Random = new();
    private static SoundEffectInstance _voice;

    public static void Init(ContentManager content)
    {
        _content = content;
        _root = Path.Combine(AppContext.BaseDirectory, content.RootDirectory);
    }

    /// <summary>Jsou všechny zvuky vypnuté (klávesa M nebo ikonka reproduktoru)?</summary>
    public static bool Muted => SoundEffect.MasterVolume <= 0f;

    public static void ToggleMute() => SoundEffect.MasterVolume = Muted ? 1f : 0f;

    /// <summary>Existuje aspoň jedna varianta tohoto zvuku?</summary>
    public static bool Exists(string name) => Variants(name).Count > 0;

    /// <param name="name">Cesta v Content bez přípony a bez čísla varianty, např. "Sounds/click_flower".</param>
    /// <param name="pitchVariation">Náhodná změna výšky (0.1 = ±10 %), aby opakované zvuky neotravovaly.</param>
    public static void Play(string name, float volume = 1f, float pitchVariation = 0f)
    {
        var sound = Pick(name);
        if (sound == null)
            return;

        sound.Play(Math.Clamp(volume, 0f, 1f), RandomPitch(pitchVariation), 0f);
    }

    /// <summary>
    /// Přehraje mluvenou repliku. Předchozí replika se utne, aby se dvě nepřekrývaly
    /// (když hráč v dialogu odklikne dřív, než záporák domluví).
    /// </summary>
    public static void PlayVoice(string name, float volume = 1f)
    {
        StopVoice();
        var sound = Pick(name);
        if (sound == null)
            return;

        _voice = sound.CreateInstance();
        _voice.Volume = Math.Clamp(volume, 0f, 1f);
        _voice.Play();
    }

    public static void StopVoice()
    {
        _voice?.Stop();
        _voice?.Dispose();
        _voice = null;
    }

    /// <summary>První nalezená varianta zvuku (pro smyčky, např. hudbu).</summary>
    public static SoundEffect GetFirst(string name)
    {
        var variants = Variants(name);
        return variants.Count > 0 ? variants[0] : null;
    }

    private static SoundEffect Pick(string name)
    {
        var variants = Variants(name);
        return variants.Count == 0 ? null : variants[Random.Next(variants.Count)];
    }

    private static float RandomPitch(float variation) =>
        variation > 0 ? (float)(Random.NextDouble() * 2 - 1) * variation : 0f;

    private static List<SoundEffect> Variants(string name)
    {
        if (string.IsNullOrEmpty(name) || _content == null)
            return new List<SoundEffect>();

        if (Cache.TryGetValue(name, out var cached))
            return cached;

        var list = new List<SoundEffect>();
        AddIfFound(list, name);
        for (int i = 1; i <= MaxVariants; i++)
            AddIfFound(list, $"{name}_{i:00}");

        // Zapamatujeme si i prázdný výsledek, ať se soubory nehledají pořád dokola.
        Cache[name] = list;
        return list;
    }

    private static void AddIfFound(List<SoundEffect> list, string name)
    {
        var sound = TryLoad(name);
        if (sound != null)
            list.Add(sound);
    }

    private static SoundEffect TryLoad(string name)
    {
        string basePath = Path.Combine(_root, name.Replace('/', Path.DirectorySeparatorChar));

        // 1) Obyčejný WAV soubor (doporučeno – stačí ho dát do složky).
        string wav = basePath + ".wav";
        if (File.Exists(wav))
        {
            try
            {
                return SoundEffect.FromFile(wav);
            }
            catch (Exception e)
            {
                // Nejčastěji špatný formát – ulož z Audacity jako WAV 16-bit PCM.
                Debug.WriteLine($"[Sounds] Nejde načíst {wav}: {e.Message}");
                return null;
            }
        }

        // 2) Zvuk přidaný postaru přes MGCB Editor (.xnb).
        if (File.Exists(basePath + ".xnb"))
        {
            try
            {
                return _content.Load<SoundEffect>(name);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[Sounds] Nejde načíst {name}.xnb: {e.Message}");
            }
        }

        return null;
    }
}

/// <summary>
/// Zvuk, který hraje pořád dokola (hudba) a plynule se zesiluje/zeslabuje
/// podle <see cref="TargetVolume"/>. Když soubor neexistuje, je potichu.
/// </summary>
public sealed class SoundLoop
{
    private readonly string _name;
    private SoundEffectInstance _instance;
    private bool _loadTried;

    public SoundLoop(string name, float fadePerSecond = 0.4f)
    {
        _name = name;
        FadePerSecond = fadePerSecond;
    }

    public string Name => _name;
    public float TargetVolume { get; set; }
    public float Volume { get; private set; }
    public float FadePerSecond { get; set; }
    public bool Exists => Sounds.Exists(_name);

    public void Update(float deltaSeconds)
    {
        float step = FadePerSecond * deltaSeconds;
        Volume = Volume < TargetVolume
            ? Math.Min(TargetVolume, Volume + step)
            : Math.Max(TargetVolume, Volume - step);

        if (Volume <= 0.001f)
        {
            if (_instance?.State == SoundState.Playing)
                _instance.Pause();
            return;
        }

        if (_instance == null)
        {
            if (_loadTried)
                return;
            _loadTried = true;

            var sound = Sounds.GetFirst(_name);
            if (sound == null)
                return;
            _instance = sound.CreateInstance();
            _instance.IsLooped = true;
        }

        _instance.Volume = Math.Clamp(Volume, 0f, 1f);
        if (_instance.State == SoundState.Paused)
            _instance.Resume();
        else if (_instance.State == SoundState.Stopped)
            _instance.Play();
    }

    public void Stop()
    {
        _instance?.Stop();
        _instance?.Dispose();
        _instance = null;
        _loadTried = false;
        Volume = 0;
        TargetVolume = 0;
    }
}
