using System.Text;
using Hudebna;

// Hudebna: složí 4 skladby do hry a ke každé podklady k nabroukání.
//   1. argument = kam uložit hotovou hudbu (Content/Music hry)
//   2. argument = kam uložit podklady k nabroukání
// Režim „zvuky“: dorobí chybějící zvukové efekty (existující nepřepisuje):
//   dotnet run --project Nastroje/Hudebna -- zvuky SimulatorCisteniVzduchu/Content/Sounds
if (args.Length > 1 && args[0] == "zvuky")
{
    Console.WriteLine("Dorábím chybějící zvuky:");
    Effects.Generate(args[1]);
    return;
}

string musicDir = args.Length > 0 ? args[0] : Path.Combine("SimulatorCisteniVzduchu", "Content", "Music");
string guideDir = args.Length > 1 ? args[1] : Path.Combine("Nastroje", "Hudebna", "podklady");

foreach (var song in Songs.All())
{
    Console.WriteLine($"{song.File}: {song.Title}, {song.Bpm} BPM, {song.LengthSeconds:0.##} s");

    // 1) Hotová skladba do hry (smyčky jsou bez cvaknutí na přechodu konec → začátek)
    var mix = Synth.Render(song.Notes, song.Bpm, song.Samples, song.Loop);
    Synth.Normalize(mix, 0.85f);
    Synth.SaveWav(Path.Combine(musicDir, song.File + ".wav"), mix);

    // 2) Podklady k nabroukání: 1 takt metronomu na rozjezd, pak skladba
    string dir = Path.Combine(guideDir, song.File);
    double countIn = song.BarSeconds;
    int guideSamples = (int)Math.Round((song.LengthSeconds + countIn + 1) * Synth.Rate);
    var metronome = song.Metronome().ToList();

    void Stem(string name, IEnumerable<Note> notes)
    {
        var buffer = Synth.Render(notes, song.Bpm, guideSamples, loop: false);
        Synth.Normalize(buffer, 0.8f);
        Synth.SaveWav(Path.Combine(dir, name + ".wav"), buffer);
    }

    // Skladba v podkladech začíná až po rozjezdovém taktu → posuneme ji o 4 doby.
    IEnumerable<Note> Shifted(IEnumerable<Note> notes) => notes.Select(n => n with { Beat = n.Beat + 4 });
    var melody = song.Notes.Where(n => n.Inst is Inst.Hum or Inst.Whistle or Inst.Brass).ToList();
    var bass = song.Notes.Where(n => n.Inst == Inst.Bass).ToList();
    var drums = song.Notes.Where(n => n.Inst is Inst.Kick or Inst.Snare or Inst.Hat).ToList();

    Stem("01_metronom", metronome);
    Stem("02_melodie", metronome.Take(4).Concat(Shifted(melody)));
    if (bass.Count > 0)
        Stem("03_basa", metronome.Take(4).Concat(Shifted(bass)));
    if (drums.Count > 0)
        Stem("04_rytmus", metronome.Take(4).Concat(Shifted(drums)));
    Stem("05_cela_skladba", metronome.Take(4).Concat(Shifted(song.Notes)));

    File.WriteAllText(Path.Combine(dir, "noty.txt"), song.Sheet(melody, bass, drums), Encoding.UTF8);
}
Console.WriteLine("Hotovo.");

/// <summary>Jedna skladba: tempo, počet taktů a noty.</summary>
public class Song
{
    public string File = "";
    public string Title = "";
    public double Bpm;
    public int Bars;
    public bool Loop = true;
    public double OneShotSeconds;
    public readonly List<Note> Notes = new();

    public double BeatSeconds => 60.0 / Bpm;
    public double BarSeconds => 4 * BeatSeconds;
    public double LengthSeconds => Loop ? Bars * BarSeconds : OneShotSeconds;
    public int Samples => (int)Math.Round(LengthSeconds * Synth.Rate);

    // ---------------------------------------------------------------- zápis not

    private static readonly string[] Names = { "C", "Cis", "D", "Dis", "E", "F", "Fis", "G", "Gis", "A", "B", "H" };

    /// <summary>
    /// Zapíše řadu not, např. "E4:2 D4:1 C4:1 | C4:2 A3:2". „-:1“ = pauza na 1 dobu.
    /// Svislá čára jen odděluje takty (pro čitelnost).
    /// </summary>
    public void Line(Inst inst, string text, double startBeat = 0, double velocity = 1)
    {
        double beat = startBeat;
        foreach (string token in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (token == "|")
                continue;
            var parts = token.Split(':');
            double length = double.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
            if (parts[0] != "-")
                Notes.Add(new Note(beat, length, ParseMidi(parts[0]), inst, velocity));
            beat += length;
        }
    }

    private static int ParseMidi(string name)
    {
        int[] semitones = { 9, 11, 0, 2, 4, 5, 7 }; // A H C D E F G
        int semi = semitones[char.ToUpperInvariant(name[0]) - 'A'];
        int i = 1;
        if (name[i] == '#') { semi++; i++; }
        else if (name[i] == 'b') { semi--; i++; }
        int octave = int.Parse(name[i..]);
        return 12 * (octave + 1) + semi;
    }

    /// <summary>Akordy: každý zní celých „beatsEach“ dob.</summary>
    public void Chords(double beatsEach, double velocity, params int[][] chords)
    {
        for (int c = 0; c < chords.Length; c++)
            foreach (int midi in chords[c])
                Notes.Add(new Note(c * beatsEach, beatsEach, midi, Inst.Pad, velocity));
    }

    /// <summary>
    /// Bicí po šestnáctinách: 16 znaků na takt, „x“ = úder, „.“ = nic.
    /// Vzor se opakuje ve všech taktech (od taktu fromBar).
    /// </summary>
    public void Drums(Inst inst, string pattern, double velocity = 1, int fromBar = 0, int toBar = -1)
    {
        if (toBar < 0) toBar = Bars;
        for (int bar = fromBar; bar < toBar; bar++)
            for (int step = 0; step < pattern.Length; step++)
                if (pattern[step] == 'x')
                    Notes.Add(new Note(bar * 4 + step * 0.25, 0.25, 0, inst, velocity));
    }

    public IEnumerable<Note> Metronome()
    {
        int bars = (Loop ? Bars : (int)Math.Ceiling(OneShotSeconds / BarSeconds)) + 1;
        for (int b = 0; b < bars * 4; b++)
            yield return new Note(b, 0.1, 0, b % 4 == 0 ? Inst.Accent : Inst.Click);
    }

    // ---------------------------------------------------------------- noty pro člověka

    public string Sheet(List<Note> melody, List<Note> bass, List<Note> drums)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Title}  –  {File}.wav");
        sb.AppendLine($"Tempo {Bpm} BPM, takt = 4 doby = {BarSeconds:0.###} s.");
        sb.AppendLine(Loop
            ? $"Smyčka {Bars} taktů = {LengthSeconds:0.###} s. V podkladech je na začátku 1 takt metronomu na rozjezd ({BarSeconds:0.###} s)."
            : $"Krátký motiv, asi {LengthSeconds:0.#} s. V podkladech je na začátku 1 takt metronomu na rozjezd ({BarSeconds:0.###} s).");
        sb.AppendLine("Délky jsou v dobách (1 = jedno ťuknutí metronomu, 0,5 = polovina). Noty jsou česky (B = b, H = h).");
        sb.AppendLine();

        void Part(string title, string how, List<Note> notes)
        {
            if (notes.Count == 0) return;
            sb.AppendLine($"== {title} ==");
            sb.AppendLine(how);
            int bars = Loop ? Bars : (int)Math.Ceiling(notes.Max(n => n.Beat + n.Length) / 4);
            for (int bar = 0; bar < bars; bar++)
            {
                var inBar = notes.Where(n => n.Beat >= bar * 4 && n.Beat < bar * 4 + 4).OrderBy(n => n.Beat).ToList();
                if (inBar.Count == 0) continue;
                var parts = inBar.Select(n => $"{Names[n.Midi % 12]} ({n.Length.ToString("0.##", System.Globalization.CultureInfo.GetCultureInfo("cs-CZ"))})");
                sb.AppendLine($"  Takt {bar + 1}: {string.Join("  ", parts)}");
            }
            sb.AppendLine();
        }

        Part("MELODIE", Loop ? "Broukej „mmm“ nebo „úúú“ (u veselé skladby pískej). Poslouchej 02_melodie.wav a kopíruj." :
            "Zlé „DAM – DAM – DAAAAM“ hlubokým hlasem.", melody);
        Part("BASA", "Hluboké „dum“ zavřenou pusou, na každou notu jedno „dum“. Vzor: 03_basa.wav.", bass);

        if (drums.Count > 0)
        {
            sb.AppendLine("== RYTMUS (beatbox) ==");
            sb.AppendLine("Každý takt = 16 políček (4 na dobu). bum = kopák (rty „b“), pš = virbl, ts = činel (zuby). Vzor: 04_rytmus.wav.");
            int bars = Loop ? Bars : 1;
            string previous = "";
            for (int bar = 0; bar < bars; bar++)
            {
                var steps = Enumerable.Repeat("·", 16).ToArray();
                foreach (var n in drums.Where(n => n.Beat >= bar * 4 && n.Beat < bar * 4 + 4))
                {
                    int step = (int)Math.Round((n.Beat - bar * 4) * 4);
                    steps[step] = n.Inst switch { Inst.Kick => "bum", Inst.Snare => "pš", _ => steps[step] == "·" ? "ts" : steps[step] };
                }
                string line = string.Join(" ", steps);
                sb.AppendLine(line == previous ? $"  Takt {bar + 1}: stejně" : $"  Takt {bar + 1}: {line}");
                previous = line;
            }
        }
        return sb.ToString();
    }
}

/// <summary>Noty všech skladeb.</summary>
public static class Songs
{
    public static IEnumerable<Song> All()
    {
        yield return Smog();
        yield return Clean();
        yield return Almost();
        yield return Villain();
    }

    /// <summary>Začátek hry: smutná zasmogovaná smyčka v a moll.</summary>
    private static Song Smog()
    {
        var s = new Song { File = "music_smog", Title = "Smog (smutná)", Bpm = 100, Bars = 8 };
        s.Line(Inst.Hum, "E4:2 D4:1 C4:1 | C4:2 A3:2 | E4:1 G4:1 E4:2 | D4:3 -:1 | E4:2 D4:1 C4:1 | A3:1 C4:1 F4:2 | E4:1 D4:1 C4:1 B3:1 | G#3:2 B3:2");
        s.Line(Inst.Bass, "A2:2 A2:2 | F2:2 F2:2 | C3:2 C3:2 | G2:2 G2:2 | A2:2 A2:2 | F2:2 F2:2 | D2:2 D2:2 | E2:2 E2:2");
        s.Chords(4, 1,
            new[] { 57, 60, 64 }, new[] { 53, 57, 60 }, new[] { 55, 60, 64 }, new[] { 55, 59, 62 },
            new[] { 57, 60, 64 }, new[] { 53, 57, 60 }, new[] { 53, 57, 62 }, new[] { 52, 56, 59 });
        s.Drums(Inst.Kick, "x.......x.......", 0.6);
        s.Drums(Inst.Hat, "....x.......x...", 0.5);
        return s;
    }

    /// <summary>Střed hry: veselá pískaná smyčka v C dur.</summary>
    private static Song Clean()
    {
        var s = new Song { File = "music_clean", Title = "Čistý vzduch (veselá)", Bpm = 120, Bars = 8 };
        s.Line(Inst.Whistle, "E5:.5 G5:.5 G5:1 E5:.5 D5:.5 C5:1 | D5:1 B4:.5 D5:.5 G5:2 | C5:.5 E5:.5 A5:1 G5:.5 E5:.5 C5:1 | F5:1 E5:.5 D5:.5 C5:2 | E5:.5 G5:.5 C6:1 B5:.5 G5:.5 E5:1 | D5:.5 E5:.5 F5:.5 G5:.5 D5:2 | A5:1 F5:1 G5:1 B4:1 | C5:3 -:1");
        s.Line(Inst.Bass, "C3:1 C3:.5 G2:.5 C3:1 E3:1 | G2:1 G2:.5 D3:.5 G2:1 B2:1 | A2:1 A2:.5 E3:.5 A2:1 C3:1 | F2:1 F2:.5 C3:.5 F2:1 A2:1 | C3:1 C3:.5 G2:.5 C3:1 E3:1 | G2:1 G2:.5 D3:.5 G2:1 B2:1 | F2:1 A2:1 G2:1 B2:1 | C3:2 G2:1 C3:1");
        s.Drums(Inst.Kick, "x.......x.x.....", 0.9);
        s.Drums(Inst.Snare, "....x.......x...", 0.8);
        s.Drums(Inst.Hat, "x.x.x.x.x.x.x.x.", 0.45);
        return s;
    }

    /// <summary>Konec hry (kyslík nad 90 %): rychlá napínavá smyčka v d moll.</summary>
    private static Song Almost()
    {
        var s = new Song { File = "music_almost", Title = "Skoro konec (napínavá)", Bpm = 150, Bars = 8 };
        s.Line(Inst.Hum, "A4:4 | A4:2 G4:1 A4:1 | A#4:4 | A#4:2 A4:1 A#4:1 | C5:4 | C5:2 A#4:1 C5:1 | C#5:4 | E5:4");
        var bassBars = new[] { "D2", "D2", "A#1", "A#1", "C2", "C2", "A1", "A1" };
        s.Line(Inst.Bass, string.Join(" | ", bassBars.Select(n => string.Join(" ", Enumerable.Repeat(n + ":.5", 8)))));
        s.Chords(8, 0.9, new[] { 50, 53, 57 }, new[] { 46, 50, 53 }, new[] { 48, 52, 55 }, new[] { 45, 49, 52 });
        s.Drums(Inst.Kick, "x...x...x...x...", 0.85);
        s.Drums(Inst.Hat, "..x...x...x...x.", 0.5);
        s.Drums(Inst.Snare, "....x.......x...", 0.6, 0, 7);
        s.Drums(Inst.Snare, "....x...x.x.xxxx", 0.7, 7, 8);   // víření před návratem na začátek
        return s;
    }

    /// <summary>Krátký zlý motiv záporáka (zazní jednou při stavbě továrny).</summary>
    private static Song Villain()
    {
        var s = new Song { File = "music_villain", Title = "Motiv záporáka", Bpm = 90, Loop = false, OneShotSeconds = 2.9 };
        s.Line(Inst.Brass, "D3:.5 D3:.5 G#2:2.5");
        s.Line(Inst.Bass, "D2:.5 D2:.5 G#1:2.5");
        s.Notes.Add(new Note(0, 0.25, 0, Inst.Kick));
        s.Notes.Add(new Note(0.5, 0.25, 0, Inst.Kick));
        s.Notes.Add(new Note(1, 0.25, 0, Inst.Kick, 1.2));
        return s;
    }
}
