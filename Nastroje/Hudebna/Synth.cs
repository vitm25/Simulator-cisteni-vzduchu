namespace Hudebna;

/// <summary>Nástroje, které napodobují zvuky z pusy (broukání, pískání, „dum“, „bum“, „ts“).</summary>
public enum Inst
{
    Hum,      // melodie broukaná „mmm / úúú“
    Whistle,  // melodie pískaná
    Pad,      // tiché dlouhé „óóó“ v akordech
    Bass,     // hluboké „dum“
    Kick,     // beatbox „bum“
    Snare,    // beatbox „pš / ka“
    Hat,      // beatbox „ts“
    Brass,    // zlé „DAAAM“ záporáka
    Click,    // metronom (jen v podkladech)
    Accent,   // metronom – první doba taktu
}

/// <summary>Jedna nota: kdy začíná (v dobách), jak dlouho trvá (v dobách), výška (MIDI), síla.</summary>
public record Note(double Beat, double Length, int Midi, Inst Inst, double Velocity = 1);

public static class Synth
{
    public const int Rate = 44100;

    public static double Freq(int midi) => 440.0 * Math.Pow(2, (midi - 69) / 12.0);

    /// <summary>Jak dlouho nástroj doznívá po konci noty (sekundy).</summary>
    public static double Release(Inst inst) => inst switch
    {
        Inst.Hum => 0.15,
        Inst.Whistle => 0.08,
        Inst.Pad => 0.6,
        Inst.Bass => 0.1,
        Inst.Brass => 0.45,
        _ => 0,
    };

    /// <summary>Pevná délka bicích a metronomu (nezávisí na délce noty).</summary>
    private static double FixedLength(Inst inst) => inst switch
    {
        Inst.Kick => 0.35,
        Inst.Snare => 0.25,
        Inst.Hat => 0.08,
        Inst.Click or Inst.Accent => 0.05,
        _ => -1,
    };

    private static double Gain(Inst inst) => inst switch
    {
        Inst.Hum => 0.50,
        Inst.Whistle => 0.42,
        Inst.Pad => 0.16,
        Inst.Bass => 0.55,
        Inst.Kick => 0.75,
        Inst.Snare => 0.35,
        Inst.Hat => 0.16,
        Inst.Brass => 0.55,
        _ => 0.6,
    };

    /// <summary>
    /// Vyrenderuje noty do zvuku. loopSamples &gt; 0 = smyčka: doznívání na konci se přičte
    /// na začátek, takže se skladba dá přehrávat dokola bez cvaknutí.
    /// </summary>
    public static float[] Render(IEnumerable<Note> notes, double bpm, int totalSamples, bool loop, double offsetSeconds = 0)
    {
        var buffer = new float[totalSamples];
        double beatSeconds = 60.0 / bpm;
        int seed = 1;
        foreach (var note in notes)
        {
            double noteSeconds = note.Length * beatSeconds;
            double fixedLength = FixedLength(note.Inst);
            double duration = fixedLength > 0 ? fixedLength : noteSeconds + Release(note.Inst);
            int start = (int)Math.Round((offsetSeconds + note.Beat * beatSeconds) * Rate);
            int count = (int)(duration * Rate);
            double f = note.Midi > 0 ? Freq(note.Midi) : 0;
            var random = new Random(seed++);
            double lastNoise = 0;
            double gain = Gain(note.Inst) * note.Velocity;

            for (int i = 0; i < count; i++)
            {
                double t = (double)i / Rate;
                double noise = random.NextDouble() * 2 - 1;
                double highNoise = noise - lastNoise;   // jednoduchý filtr: jen vysoké „sss“
                lastNoise = noise;

                double s = Sample(note.Inst, t, noteSeconds, f, highNoise) * gain;
                int index = start + i;
                if (loop)
                    index %= totalSamples;
                else if (index >= totalSamples)
                    break;
                buffer[index] += (float)s;
            }
        }
        return buffer;
    }

    private static double Sample(Inst inst, double t, double len, double f, double noise)
    {
        switch (inst)
        {
            case Inst.Hum:
            {
                double env = Env(t, len, 0.06, 0.15);
                double p = VibratoPhase(t, f, 0.004, 5.0);
                return env * (Math.Sin(p) + 0.35 * Math.Sin(2 * p) + 0.15 * Math.Sin(3 * p) + 0.06 * Math.Sin(4 * p)) / 1.56;
            }
            case Inst.Whistle:
            {
                double env = Env(t, len, 0.02, 0.08);
                double p = VibratoPhase(t, f, 0.006, 6.0);
                return env * (Math.Sin(p) + 0.05 * Math.Sin(2 * p)) / 1.05;
            }
            case Inst.Pad:
            {
                double env = Env(t, len, 0.35, 0.6);
                double p = VibratoPhase(t, f, 0.003, 4.2);
                return env * (Math.Sin(p) + 0.25 * Math.Sin(2 * p) + 0.08 * Math.Sin(3 * p)) / 1.33;
            }
            case Inst.Bass:
            {
                double env = Env(t, len, 0.01, 0.1) * Math.Exp(-t / 0.5);
                // „dum“ na začátku trochu spadne z výšky
                double p = 2 * Math.PI * (f * t + f * 0.08 * 0.03 * (1 - Math.Exp(-t / 0.03)));
                return env * (Math.Sin(p) + 0.25 * Math.Sin(2 * p)) / 1.25;
            }
            case Inst.Kick:
            {
                double p = 2 * Math.PI * (45 * t + 85 * 0.03 * (1 - Math.Exp(-t / 0.03)));
                return Math.Sin(p) * Math.Exp(-t / 0.16) * Math.Min(1, t / 0.002);
            }
            case Inst.Snare:
                return noise * 0.55 * Math.Exp(-t / 0.08) + Math.Sin(2 * Math.PI * 185 * t) * 0.45 * Math.Exp(-t / 0.05);
            case Inst.Hat:
                return noise * Math.Exp(-t / 0.025);
            case Inst.Brass:
            {
                double env = Env(t, len, 0.03, 0.45);
                double p = VibratoPhase(t, f, t > 0.35 ? 0.006 : 0, 5.5);
                double s = 0;
                for (int k = 1; k <= 8; k++)
                    s += Math.Sin(k * p) / Math.Pow(k, 1.2);
                return env * Math.Tanh(s * 1.4) / 1.1;
            }
            case Inst.Click:
                return Math.Sin(2 * Math.PI * 1500 * t) * Math.Exp(-t / 0.012);
            case Inst.Accent:
                return Math.Sin(2 * Math.PI * 2200 * t) * Math.Exp(-t / 0.015);
            default:
                return 0;
        }
    }

    /// <summary>Náběh, držení a doznění noty.</summary>
    private static double Env(double t, double len, double attack, double release)
    {
        double a = Math.Min(1, t / attack);
        if (t <= len)
            return a;
        double r = 1 - (t - len) / release;
        return r <= 0 ? 0 : a * r;
    }

    /// <summary>Fáze tónu s vibratem (frekvence se lehce vlní jako u hlasu).</summary>
    private static double VibratoPhase(double t, double f, double depth, double rate) =>
        2 * Math.PI * f * t - (f * depth / rate) * Math.Cos(2 * Math.PI * rate * t);

    /// <summary>Ztiší/zesílí zvuk tak, aby nejhlasitější místo bylo na dané úrovni.</summary>
    public static void Normalize(float[] buffer, float peak = 0.85f)
    {
        float max = 0;
        foreach (float s in buffer)
            max = Math.Max(max, Math.Abs(s));
        if (max <= 0)
            return;
        float k = peak / max;
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] *= k;
    }

    /// <summary>Uloží WAV 16-bit PCM, mono, 44 100 Hz (formát, který hra umí).</summary>
    public static void SaveWav(string path, float[] samples)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        using var w = new BinaryWriter(File.Create(path));
        int dataBytes = samples.Length * 2;
        w.Write("RIFF"u8);
        w.Write(36 + dataBytes);
        w.Write("WAVE"u8);
        w.Write("fmt "u8);
        w.Write(16);
        w.Write((short)1);        // PCM
        w.Write((short)1);        // mono
        w.Write(Rate);
        w.Write(Rate * 2);
        w.Write((short)2);
        w.Write((short)16);
        w.Write("data"u8);
        w.Write(dataBytes);
        foreach (float s in samples)
            w.Write((short)Math.Clamp((int)Math.Round(s * 32767), -32768, 32767));
    }
}
