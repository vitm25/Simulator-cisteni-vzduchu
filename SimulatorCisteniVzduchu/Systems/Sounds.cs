using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace SimulatorCisteniVzduchu.Systems;

/// <summary>
/// Přehrávání zvuků podle jména, např. Sounds.Play("Sounds/villain_laugh").
/// Když zvuk ještě není nahraný a přidaný v MGCB Editoru, nic se nestane –
/// hra tak funguje i bez zvuků a ty je můžeš přidávat postupně.
/// </summary>
public static class Sounds
{
    private static ContentManager _content;
    private static readonly Dictionary<string, SoundEffect> Cache = new();
    private static readonly Random Random = new();

    public static void Init(ContentManager content) => _content = content;

    /// <param name="name">Cesta v Content bez přípony, např. "Sounds/click_flower_01".</param>
    /// <param name="pitchVariation">Náhodná změna výšky (0.1 = ±10 %), aby opakované zvuky neotravovaly.</param>
    public static void Play(string name, float volume = 1f, float pitchVariation = 0f)
    {
        var sound = Get(name);
        if (sound == null)
            return;

        float pitch = pitchVariation > 0 ? (float)(Random.NextDouble() * 2 - 1) * pitchVariation : 0f;
        sound.Play(volume, pitch, 0f);
    }

    private static SoundEffect Get(string name)
    {
        if (string.IsNullOrEmpty(name) || _content == null)
            return null;

        if (Cache.TryGetValue(name, out var cached))
            return cached;

        SoundEffect sound = null;
        try
        {
            sound = _content.Load<SoundEffect>(name);
        }
        catch (ContentLoadException)
        {
            // Zvuk zatím neexistuje – zapamatujeme si to, ať to nezkoušíme pořád dokola.
        }

        Cache[name] = sound;
        return sound;
    }
}
