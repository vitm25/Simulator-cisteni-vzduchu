using System;

namespace SimulatorCisteniVzduchu.Systems;

/// <summary>
/// Hudba a zvuky prostředí během hry:
///   - hudba se mění podle kyslíku (music_smog → music_clean → music_almost) a plynule se prolíná,
///   - čas od času zafouká vítr (čím víc smogu, tím silněji),
///   - když je vzduch čistší, ozvou se ptáci.
/// Chybějící soubory se prostě přeskočí.
/// </summary>
public class Ambience
{
    private const float MusicVolume = 0.45f;

    /// <summary>Při dialogu se hudba ztiší, aby byl slyšet záporák.</summary>
    private const float DuckedFactor = 0.35f;

    // Pořadí = od nejšpinavějšího vzduchu po nejčistší.
    private readonly SoundLoop[] _music =
    {
        new("Music/music_smog"),
        new("Music/music_clean"),
        new("Music/music_almost"),
    };

    private readonly Random _random = new();
    private double _windTimer = 3;
    private double _birdsTimer = 8;

    /// <param name="oxygen">Kyslík v procentech.</param>
    /// <param name="progress">Postup ke konci hry 0..1.</param>
    /// <param name="ducked">Probíhá dialog → hudba potichu.</param>
    public void Update(float deltaSeconds, double oxygen, double progress, bool ducked)
    {
        int wanted = oxygen < 50 ? 0 : oxygen < 90 ? 1 : 2;
        int track = PickExistingTrack(wanted);

        for (int i = 0; i < _music.Length; i++)
        {
            _music[i].TargetVolume = i == track ? MusicVolume * (ducked ? DuckedFactor : 1f) : 0f;
            _music[i].Update(deltaSeconds);
        }

        if (ducked)
            return;

        _windTimer -= deltaSeconds;
        if (_windTimer <= 0)
        {
            _windTimer = 6 + _random.NextDouble() * 8;
            Sounds.Play("Sounds/wind", 0.15f + 0.45f * (float)(1 - progress), 0.1f);
        }

        if (oxygen >= 50)
        {
            _birdsTimer -= deltaSeconds;
            if (_birdsTimer <= 0)
            {
                _birdsTimer = 8 + _random.NextDouble() * 10;
                float loudness = (float)Math.Clamp((oxygen - 50) / 40, 0, 1);
                Sounds.Play("Sounds/birds", 0.15f + 0.35f * loudness, 0.08f);
            }
        }
    }

    public void Stop()
    {
        foreach (var loop in _music)
            loop.Stop();
    }

    /// <summary>
    /// Když chtěná skladba ještě není nahraná, hraje nejbližší, která existuje
    /// (nejdřív ty „špinavější“), ať hudba neztichne.
    /// </summary>
    private int PickExistingTrack(int wanted)
    {
        if (_music[wanted].Exists)
            return wanted;
        for (int i = wanted - 1; i >= 0; i--)
            if (_music[i].Exists)
                return i;
        for (int i = wanted + 1; i < _music.Length; i++)
            if (_music[i].Exists)
                return i;
        return -1;
    }
}
