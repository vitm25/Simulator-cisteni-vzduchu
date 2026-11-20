namespace Hudebna;

/// <summary>
/// Dorobí chybějící zvukové efekty. Kde to jde, poskládá je z nahrávek uživatele
/// (smích z dialogu, vítr, „BUUM“), zbytek dosyntetizuje.
/// Existující soubory NIKDY nepřepisuje.
/// </summary>
public static class Effects
{
    private const int Rate = Synth.Rate;
    private static readonly Random Rnd = new(7);

    public static void Generate(string soundsDir)
    {
        string Src(string name) => Path.Combine(soundsDir, name + ".wav");

        Make(soundsDir, "birds", Birds);
        Make(soundsDir, "bees", Bees);
        Make(soundsDir, "robot_work", Robot);
        Make(soundsDir, "greenhouse", Greenhouse);
        Make(soundsDir, "tractor_field", Tractor);
        Make(soundsDir, "drone", Drone);
        Make(soundsDir, "villain_laugh", () => Cut(Read(Src("dialog_00_5")), 3.25, 4.17));
        Make(soundsDir, "villain_factory", () => VillainFactory(Read(Src("planet_explosion"))));
        Make(soundsDir, "factory_sabotage", () => Sabotage(Read(Src("planet_explosion"))));
        Make(soundsDir, "ending_silence_wind", () => SpaceWind(Read(Src("wind_01"))));

        // smog_collect hraje při každém zničení továrny – nesmí trvat desítky sekund.
        TrimIfTooLong(Src("smog_collect"), maxSeconds: 3, keepSeconds: 0.6);
    }

    /// <summary>Zkrátí nahrávku, která je omylem moc dlouhá (za zvukem bylo jen ticho).</summary>
    private static void TrimIfTooLong(string path, double maxSeconds, double keepSeconds)
    {
        if (!File.Exists(path))
            return;
        var x = Read(path);
        if (x.Length <= maxSeconds * Rate)
            return;
        var y = Cut(x, 0, keepSeconds);
        Synth.SaveWav(path, y);
        Console.WriteLine($"  {Path.GetFileNameWithoutExtension(path),-22} zkráceno z {x.Length / (double)Rate:0.0} s na {keepSeconds} s");
    }

    private static void Make(string dir, string name, Func<float[]> build)
    {
        string path = Path.Combine(dir, name + ".wav");
        if (File.Exists(path))
        {
            Console.WriteLine($"  {name,-22} už existuje – nepřepisuju");
            return;
        }
        var samples = build();
        Synth.Normalize(samples, 0.8f);
        Synth.SaveWav(path, samples);
        Console.WriteLine($"  {name,-22} {samples.Length / (double)Rate:0.00} s");
    }

    // ------------------------------------------------------------ z nahrávek uživatele

    /// <summary>Načte WAV 16-bit mono (formát, který z Audacity ukládáš).</summary>
    public static float[] Read(string path)
    {
        var b = File.ReadAllBytes(path);
        int pos = 12;
        while (pos + 8 <= b.Length)
        {
            string id = System.Text.Encoding.ASCII.GetString(b, pos, 4);
            int size = BitConverter.ToInt32(b, pos + 4);
            if (id == "data")
            {
                int n = Math.Min(size, b.Length - pos - 8) / 2;
                var x = new float[n];
                for (int i = 0; i < n; i++)
                    x[i] = BitConverter.ToInt16(b, pos + 8 + i * 2) / 32768f;
                return x;
            }
            pos += 8 + size + (size & 1);
        }
        throw new InvalidDataException("Ve WAV souboru chybí data: " + path);
    }

    /// <summary>Vyřízne úsek (sekundy) s krátkým náběhem a dozněním, aby necvakal.</summary>
    private static float[] Cut(float[] x, double from, double to)
    {
        int a = Math.Clamp((int)(from * Rate), 0, x.Length), b = Math.Clamp((int)(to * Rate), a, x.Length);
        var y = x[a..b];
        Fade(y, 0.02, 0.05);
        return y;
    }

    private static void Fade(float[] y, double fadeIn, double fadeOut)
    {
        int fi = (int)(fadeIn * Rate), fo = (int)(fadeOut * Rate);
        for (int i = 0; i < y.Length; i++)
        {
            double g = 1;
            if (i < fi) g = (double)i / fi;
            if (y.Length - i < fo) g = Math.Min(g, (double)(y.Length - i) / fo);
            y[i] *= (float)g;
        }
    }

    /// <summary>Změní výšku i délku najednou (jako zrychlená/zpomalená páska). factor &gt; 1 = výš a kratší.</summary>
    private static float[] Resample(float[] x, double factor)
    {
        int n = (int)(x.Length / factor);
        var y = new float[n];
        for (int i = 0; i < n; i++)
        {
            double p = i * factor;
            int k = (int)p;
            double t = p - k;
            float a = x[Math.Min(k, x.Length - 1)], b = x[Math.Min(k + 1, x.Length - 1)];
            y[i] = (float)(a + (b - a) * t);
        }
        return y;
    }

    private static void MixInto(float[] target, float[] source, int offset, float gain)
    {
        for (int i = 0; i < source.Length && offset + i < target.Length; i++)
            if (offset + i >= 0)
                target[offset + i] += source[i] * gain;
    }

    private static float[] VillainFactory(float[] boom)
    {
        // „BUM“ z tvé nahrávky výbuchu + syčení páry „sssss“
        var y = new float[(int)(2.0 * Rate)];
        var hit = Cut(boom, 0, 0.5);
        MixInto(y, hit, 0, 1f);
        double last = 0;
        for (int i = (int)(0.15 * Rate); i < y.Length; i++)
        {
            double t = (double)i / Rate - 0.15;
            double noise = Rnd.NextDouble() * 2 - 1;
            double hp = noise - last;
            last = noise;
            double env = Math.Min(1, t / 0.1) * Math.Exp(-t / 0.7);
            y[i] += (float)(hp * env * 0.35);
        }
        Fade(y, 0.002, 0.2);
        return y;
    }

    private static float[] Sabotage(float[] boom)
    {
        // Krátké vyšší „BUM“ z tvé nahrávky + kovové zařinčení
        var y = new float[(int)(1.0 * Rate)];
        var hit = Resample(Cut(boom, 0, 0.7), 1.5);
        MixInto(y, hit, 0, 1f);
        double[] partials = { 430, 1170, 1830, 2650 };
        for (int i = 0; i < y.Length; i++)
        {
            double t = (double)i / Rate;
            double metal = 0;
            foreach (double f in partials)
                metal += Math.Sin(2 * Math.PI * f * t) / partials.Length;
            double noise = (Rnd.NextDouble() * 2 - 1) * Math.Exp(-t / 0.06);
            y[i] += (float)((metal * Math.Exp(-t / 0.25) + noise) * 0.45);
        }
        Fade(y, 0.002, 0.15);
        return y;
    }

    private static float[] SpaceWind(float[] wind)
    {
        // Tvůj vítr, zpomalený (hlubší) a vrstvený přes sebe do 10s smyčky bez švu.
        var slow = Cut(wind, 0.05, 2.0);
        slow = Resample(slow, 0.6);
        Fade(slow, 0.6, 0.8);
        int length = 10 * Rate;
        var y = new float[length];
        for (int k = 0; k < 7; k++)
        {
            int offset = (int)(k * length / 7.0 + Rnd.Next(-Rate / 3, Rate / 3));
            for (int i = 0; i < slow.Length; i++)
                y[((offset + i) % length + length) % length] += slow[i] * 0.6f;
        }
        return y;
    }

    // ------------------------------------------------------------ syntéza

    private static float[] Buffer(double seconds) => new float[(int)(seconds * Rate)];

    private static double Env(double t, double len, double attack, double release) =>
        Math.Min(1, t / attack) * Math.Clamp((len - t) / release, 0, 1);

    private static float[] Birds()
    {
        // Pískané „fí-fíí, fí-fí-fíí“: rychlé sklouznutí nahoru s trylkem
        var y = Buffer(2.6);
        (double start, double len, double f0, double f1)[] chirps =
        {
            (0.10, 0.12, 2600, 3600), (0.28, 0.20, 2800, 4100),
            (0.95, 0.10, 3000, 3700), (1.10, 0.10, 3000, 3700), (1.25, 0.26, 2700, 4300),
            (1.95, 0.14, 3300, 2600),
        };
        foreach (var (start, len, f0, f1) in chirps)
        {
            double phase = 0;
            for (int i = 0; i < (int)(len * Rate); i++)
            {
                double t = (double)i / Rate, u = t / len;
                double f = f0 + (f1 - f0) * Math.Sin(u * Math.PI / 2) + 180 * Math.Sin(2 * Math.PI * 28 * t);
                phase += 2 * Math.PI * f / Rate;
                int idx = (int)(start * Rate) + i;
                if (idx < y.Length)
                    y[idx] += (float)(Math.Sin(phase) * Env(t, len, 0.01, 0.04) * 0.6);
            }
        }
        return y;
    }

    private static float[] Bees()
    {
        // „Bzzzz“: drsný tón, který se třese a přilétá/odlétá
        var y = Buffer(2.2);
        double phase = 0;
        for (int i = 0; i < y.Length; i++)
        {
            double t = (double)i / Rate;
            double f = 230 * (1 + 0.03 * Math.Sin(2 * Math.PI * 31 * t) + 0.05 * Math.Sin(2 * Math.PI * 0.8 * t));
            phase += 2 * Math.PI * f / Rate;
            double s = 0;
            for (int k = 1; k <= 10; k++)
                s += Math.Sin(k * phase) / k;
            double tremolo = 0.75 + 0.25 * Math.Sin(2 * Math.PI * 11 * t);
            y[i] = (float)(s * tremolo * Env(t, 2.2, 0.25, 0.5) * 0.4);
        }
        return y;
    }

    private static float[] Robot()
    {
        // „Píp-píp“ a krátké „vrrr“ motorku
        var y = Buffer(1.3);
        void Beep(double start, double f, double len)
        {
            for (int i = 0; i < (int)(len * Rate); i++)
            {
                double t = (double)i / Rate;
                y[(int)(start * Rate) + i] += (float)(Math.Sign(Math.Sin(2 * Math.PI * f * t)) * 0.25 * Env(t, len, 0.005, 0.02));
            }
        }
        Beep(0.0, 880, 0.10);
        Beep(0.16, 1320, 0.12);
        double phase = 0;
        for (int i = 0; i < (int)(0.75 * Rate); i++)
        {
            double t = (double)i / Rate;
            double f = 85 + 60 * t;
            phase += 2 * Math.PI * f / Rate;
            double s = Math.Sin(phase) + 0.5 * Math.Sin(2 * phase) + 0.33 * Math.Sin(3 * phase);
            y[(int)(0.42 * Rate) + i] += (float)(s * (0.6 + 0.4 * Math.Sin(2 * Math.PI * 22 * t)) * Env(t, 0.75, 0.05, 0.2) * 0.35);
        }
        return y;
    }

    private static float[] Greenhouse()
    {
        // Cinknutí o sklo + tichý větráček
        var y = Buffer(1.6);
        double[] partials = { 2093, 2890, 4180, 5410 };
        double last = 0;
        for (int i = 0; i < y.Length; i++)
        {
            double t = (double)i / Rate;
            double glass = 0;
            for (int k = 0; k < partials.Length; k++)
                glass += Math.Sin(2 * Math.PI * partials[k] * t) * Math.Exp(-t / (0.9 - k * 0.18)) / (k + 1);
            double noise = Rnd.NextDouble() * 2 - 1;
            double lp = last + (noise - last) * 0.05;   // tlumený šum = vzduch z větráku
            last = lp;
            double fan = lp * (0.7 + 0.3 * Math.Sin(2 * Math.PI * 14 * t)) * Env(t, 1.6, 0.3, 0.5);
            y[i] = (float)(glass * 0.5 * Math.Min(1, t / 0.002) + fan * 1.6);
        }
        return y;
    }

    private static float[] Tractor()
    {
        // „Brrm-brrm“: pravidelné bouchání motoru, které se rozjíždí
        var y = Buffer(2.2);
        double next = 0.05, rate = 8;
        while (next < 2.0)
        {
            int start = (int)(next * Rate);
            for (int i = 0; i < (int)(0.09 * Rate) && start + i < y.Length; i++)
            {
                double t = (double)i / Rate;
                double thump = Math.Sin(2 * Math.PI * (62 * t + 40 * 0.02 * (1 - Math.Exp(-t / 0.02))));
                double grit = (Rnd.NextDouble() * 2 - 1) * 0.35;
                y[start + i] += (float)((thump + grit) * Math.Exp(-t / 0.035) * 0.7);
            }
            next += 1 / rate;
            rate = Math.Min(15, rate + 0.45);
        }
        Fade(y, 0.01, 0.3);
        return y;
    }

    private static float[] Drone()
    {
        // „Vííííí“ vrtulek, přiletí a odletí (výška se lehce mění jako u projíždějícího auta)
        var y = Buffer(2.2);
        double p1 = 0, p2 = 0;
        for (int i = 0; i < y.Length; i++)
        {
            double t = (double)i / Rate, u = t / 2.2;
            double f = 330 * (1 + 0.06 * Math.Cos(Math.PI * u));
            p1 += 2 * Math.PI * f / Rate;
            p2 += 2 * Math.PI * f * 1.013 / Rate;
            double s = 0;
            for (int k = 1; k <= 6; k++)
                s += (Math.Sin(k * p1) + Math.Sin(k * p2)) / (k * 1.5);
            double loud = Math.Sin(Math.PI * u);
            y[i] = (float)(s * loud * (0.85 + 0.15 * Math.Sin(2 * Math.PI * 47 * t)) * 0.25);
        }
        return y;
    }
}
