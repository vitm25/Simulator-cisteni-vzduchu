using System.Drawing;
using Kreslirna;
using static Kreslirna.Art;

// Kam ukládat: složka Content hry (Sprites/ a Ending/ se vytvoří uvnitř).
string content = args.Length > 0 ? args[0] : Path.Combine("SimulatorCisteniVzduchu", "Content");
string sprites = Path.Combine(content, "Sprites");
string ending = Path.Combine(content, "Ending");

Console.WriteLine("Kreslím postavy a objekty:");
VillainPortrait(Path.Combine(sprites, "villain_portrait.png"));
Factory(Path.Combine(sprites, "factory.png"));
FlowerSprite(Path.Combine(sprites, "flower.png"));
Tree(Path.Combine(sprites, "tree.png"));
Greenhouse(Path.Combine(sprites, "greenhouse.png"));
Field(Path.Combine(sprites, "field.png"));
Robot(Path.Combine(sprites, "robot.png"));
Drone(Path.Combine(sprites, "drone.png"));
Bee(Path.Combine(sprites, "bee.png"));
SmogCloud(Path.Combine(sprites, "smog_cloud.png"));
SoundIcon(Path.Combine(sprites, "icon_sound_on.png"), on: true);
SoundIcon(Path.Combine(sprites, "icon_sound_off.png"), on: false);

Console.WriteLine("Kreslím pozadí:");
Background(Path.Combine(sprites, "bg_sky_smog.png"), clean: false);
Background(Path.Combine(sprites, "bg_sky_clean.png"), clean: true);

Console.WriteLine("Kreslím koncovku:");
Ending1(Path.Combine(ending, "ending_01.png"));
Ending2(Path.Combine(ending, "ending_02.png"));
Ending3(Path.Combine(ending, "ending_03.png"));
Ending4(Path.Combine(ending, "ending_04.png"));
Ending5(Path.Combine(ending, "ending_05.png"));
Console.WriteLine("Hotovo.");

// =====================================================================
//  POSTAVY A OBJEKTY (průhledné pozadí)
// =====================================================================

static void VillainPortrait(string path)
{
    using var a = new Art(230, 230, 1);
    // Oblek, košile, kravata, odznak firmy
    a.Shape(Pts(22, 229, 208, 229, 184, 176, 46, 176), Pal.Hex("34343F"), 4);
    a.Shape(Pts(95, 176, 135, 176, 115, 210), Pal.White, 3);
    a.Shape(Pts(108, 180, 122, 180, 128, 210, 115, 226, 102, 210), Pal.Red, 3);
    a.Shape(Ell(170, 196, 10, 10), Pal.Gold, 3);
    a.Text("Č", 170, 197, 13, Pal.DarkRed);
    Parts.VillainHead(a, 115, 112, 1.0f);
    a.Save(path);
}

static void Factory(string path)
{
    using var a = new Art(170, 240, 2);
    a.Amp = 1.2f;
    // Komín až k hornímu okraji – odsud hra pouští kouř
    a.Shape(Rect(116, 4, 32, 124), Pal.Gray, 4);
    a.Shape(Rect(116, 16, 32, 10), Pal.Red, 0);
    a.Shape(Rect(116, 40, 32, 10), Pal.Red, 0);
    a.Stroke(Pal.Black, 3, 116, 4, 148, 4);
    a.Spray(132, 8, 16, Pal.Hex("3A3A3A"), 120);

    // Hala se zubatou střechou
    a.Shape(Pts(4, 236, 4, 124, 42, 98, 42, 124, 80, 98, 80, 124, 166, 124, 166, 236), Pal.Brown, 4);
    for (int row = 0; row < 5; row++)
    {
        float y = 136 + row * 20;
        for (int col = 0; col < 4; col++)
        {
            float x = 12 + col * 40 + (row % 2) * 18;
            if (x < 158)
                a.Stroke(Pal.Hex("8C5A3C"), 2, x, y, x + 14, y);
        }
    }
    a.Shape(Rect(10, 138, 104, 26), Pal.White, 3);
    a.Text("Čmoud a.s.", 62, 151, 15, Pal.Black);
    a.Shape(Rect(14, 178, 24, 20), Pal.Yellow, 3);
    a.Shape(Rect(50, 178, 24, 20), Pal.Yellow, 3);
    a.Shape(Rect(128, 136, 26, 20), Pal.Yellow, 3);
    a.Shape(Rect(106, 190, 36, 46), Pal.Hex("6B3E26"), 3);
    a.Shape(Ell(134, 214, 3, 3), Pal.Gold, 0);
    a.Spray(30, 230, 26, Pal.Hex("5A3A28"), 90);
    a.Save(path);
}

static void FlowerSprite(string path)
{
    using var a = new Art(300, 530, 3);
    Parts.Flower(a, 150, 526, 1.0f, Parts.Face.Smile);
    // Trs trávy u paty
    for (int i = 0; i < 7; i++)
    {
        float x = 118 + i * 11;
        a.Stroke(Pal.Hex("17803A"), 4, x, 528, x + (i - 3) * 3, 506 - (i % 3) * 6);
    }
    a.Save(path);
}

static void Tree(string path)
{
    using var a = new Art(110, 160, 4);
    a.Amp = 1.2f;
    a.Shape(Pts(46, 158, 48, 96, 62, 96, 66, 158), Pal.Brown, 3);
    a.Stroke(Pal.Hex("8C5A3C"), 2, 54, 150, 55, 120);
    a.Stroke(Pal.Hex("8C5A3C"), 2, 58, 112, 59, 100);
    a.Shape(Ell(30, 78, 27, 26), Pal.Green, 3);
    a.Shape(Ell(80, 78, 27, 26), Pal.Green, 3);
    a.Shape(Ell(55, 48, 40, 38), Pal.Green, 3);
    a.Shape(Ell(55, 82, 30, 18), Pal.Green, 0); // zakryje vnitřní čáry
    a.Spray(66, 66, 26, Pal.Hex("17803A"), 160);
    a.Spray(40, 40, 16, Pal.Lime, 60);
    a.Shape(Ell(36, 70, 5, 5), Pal.Red, 2);
    a.Shape(Ell(76, 52, 5, 5), Pal.Red, 2);
    a.Shape(Ell(62, 86, 5, 5), Pal.Red, 2);
    a.Save(path);
}

static void Greenhouse(string path)
{
    using var a = new Art(240, 110, 5);
    a.Amp = 1.2f;
    a.Shape(Pts(3, 34, 120, 4, 237, 34), Pal.LightTurquoise, 4);
    a.Shape(Rect(3, 34, 234, 73), Pal.Hex("D6F1FA"), 4);
    // Rostlinky uvnitř
    for (int i = 0; i < 7; i++)
    {
        float x = 20 + i * 33;
        a.Stroke(Pal.Green, 3, x, 104, x, 82);
        a.Shape(Ell(x - 6, 86, 7, 4, -30), Pal.Green, 2);
        a.Shape(Ell(x + 6, 80, 7, 4, 30), Pal.Green, 2);
        if (i % 2 == 0)
            a.Shape(Ell(x + 4, 92, 5, 5), Pal.Red, 2);
    }
    // Rám
    foreach (float x in new[] { 61f, 120f, 179f })
        a.Stroke(Pal.Black, 3, x, 34, x, 106);
    a.Stroke(Pal.Black, 3, 61, 34, 120, 4);
    a.Stroke(Pal.Black, 3, 179, 34, 120, 4);
    // Odlesky skla
    a.Stroke(Pal.White, 4, 14, 70, 34, 46);
    a.Stroke(Pal.White, 4, 132, 68, 150, 46);
    a.Stroke(Pal.White, 3, 86, 26, 100, 16);
    a.Save(path);
}

static void Field(string path)
{
    using var a = new Art(260, 90, 12);
    a.Amp = 1.2f;
    // Zoraná hlína s brázdami
    a.Shape(Pts(14, 40, 246, 40, 258, 88, 2, 88), Pal.Hex("8B5A3C"), 4);
    for (int i = 0; i < 4; i++)
    {
        float y = 50 + i * 10;
        a.Stroke(Pal.Hex("6B3E26"), 3, 12 - i * 3, y, 248 + i * 3, y);
    }

    // Slunečnice: zadní řada menší, přední větší
    void Sunflower(float x, float baseY, float s)
    {
        a.Stroke(Pal.Black, 6 * s, x, baseY, x, baseY - 34 * s);
        a.Stroke(Pal.Green, 3 * s, x, baseY, x, baseY - 34 * s);
        a.Shape(Ell(x - 6 * s, baseY - 16 * s, 6 * s, 3 * s, -30), Pal.Green, 1.5f * s);
        a.Shape(Ell(x, baseY - 40 * s, 11 * s, 11 * s), Pal.Yellow, 2 * s, Pal.Black);
        a.Shape(Ell(x, baseY - 40 * s, 5 * s, 5 * s), Pal.Hex("6B3E26"), 1.5f * s);
    }
    for (int i = 0; i < 9; i++)
        Sunflower(24 + i * 27, 56, 0.85f);
    for (int i = 0; i < 8; i++)
        Sunflower(36 + i * 27, 84, 1.0f);
    a.Save(path);
}

static void Robot(string path)
{
    using var a = new Art(50, 50, 6);
    a.Amp = 0.4f;
    a.Stroke(Pal.Black, 2, 25, 4, 25, 1);
    a.Shape(Rect(15, 4, 20, 13), Pal.LightGray, 2);
    a.Shape(Ell(25, 10, 4, 4), Pal.LightTurquoise, 1.5f);
    a.Shape(Rect(9, 17, 32, 22), Pal.BlueGray, 2);
    a.Shape(Rect(14, 22, 10, 6), Pal.Yellow, 1.5f);
    // Ruka s konvičkou
    a.Stroke(Pal.Black, 2, 41, 24, 46, 20);
    a.Shape(Rect(42, 14, 7, 7), Pal.Green, 1.5f);
    a.Shape(Ell(16, 43, 5, 5), Pal.Hex("333333"), 2);
    a.Shape(Ell(34, 43, 5, 5), Pal.Hex("333333"), 2);
    a.Save(path);
}

static void Drone(string path)
{
    using var a = new Art(80, 25, 7);
    a.Amp = 0.4f;
    a.Stroke(Pal.Black, 2, 13, 6, 28, 13);
    a.Stroke(Pal.Black, 2, 67, 6, 52, 13);
    a.Shape(Ell(13, 5, 12, 3), Pal.LightGray, 1.5f);
    a.Shape(Ell(67, 5, 12, 3), Pal.LightGray, 1.5f);
    a.Shape(Rect(24, 10, 32, 11), Pal.Hex("3C3C46"), 2);
    a.Shape(Ell(40, 15, 3, 3), Pal.LightTurquoise, 0);
    a.Shape(Rect(36, 21, 8, 3), Pal.Gray, 1);
    a.Save(path);
}

static void Bee(string path)
{
    using var a = new Art(25, 25, 8);
    a.Amp = 0.2f;
    a.Shape(Ell(9, 7, 5, 4), Pal.White, 1);
    a.Shape(Ell(15, 6, 5, 4), Pal.LightTurquoise, 1);
    a.Shape(Ell(12, 15, 9, 6), Pal.Yellow, 1.5f);
    a.Stroke(Pal.Black, 2, 10, 10, 10, 20);
    a.Stroke(Pal.Black, 2, 14, 10, 14, 20);
    a.Shape(Pts(21, 14, 24, 15, 21, 17), Pal.Black, 0);
    a.Shape(Rect(4, 13, 2, 2), Pal.Black, 0);
    a.Save(path);
}

static void SoundIcon(string path, bool on)
{
    using var a = new Art(48, 48, on ? 13 : 14);
    a.Amp = 0.4f;
    // Reproduktor
    a.Shape(Rect(4, 17, 10, 14), Pal.LightGray, 2);
    a.Shape(Pts(14, 17, 27, 6, 27, 42, 14, 31), Pal.LightGray, 2);
    a.Spray(22, 30, 5, Pal.Gray, 25);
    if (on)
    {
        // Zvukové vlny
        a.OutlinedStroke(Bezier(new(32, 17), new(36, 20), new(36, 28), new(32, 31), 10), Pal.Lime, 3, 1);
        a.OutlinedStroke(Bezier(new(37, 11), new(44, 17), new(44, 31), new(37, 37), 12), Pal.Lime, 3, 1);
    }
    else
    {
        // Červený křížek
        a.OutlinedStroke(Pts(32, 16, 45, 32), Pal.Red, 4, 1);
        a.OutlinedStroke(Pts(45, 16, 32, 32), Pal.Red, 4, 1);
    }
    a.Save(path);
}

static void SmogCloud(string path)
{
    using var a = new Art(190, 115, 9);
    var fill = Pal.Hex("6E655A");
    var line = Pal.Hex("4A4238");
    a.Shape(Ell(95, 56, 60, 44), fill, 4, line);
    a.Shape(Ell(148, 66, 36, 30), fill, 4, line);
    a.Shape(Ell(42, 70, 34, 28), fill, 4, line);
    a.Shape(Ell(95, 66, 52, 32), fill, 0);
    a.Shape(Ell(128, 70, 26, 20), fill, 0);
    a.Shape(Ell(62, 72, 24, 18), fill, 0);
    a.Spray(100, 70, 40, line, 200);
    a.Spray(80, 40, 20, Pal.Hex("857B6E"), 80);
    a.Save(path);
}

// =====================================================================
//  POZADÍ 1920×1080 (zem začíná na y = 820)
// =====================================================================

static void Background(string path, bool clean)
{
    using var a = new Art(1920, 1080, clean ? 11 : 10);
    a.Amp = 3;
    var sky = clean ? Pal.Hex("7FCDF0") : Pal.Hex("8F887B");
    a.G.Clear(sky);
    a.Streaks(new RectangleF(0, 0, 1920, 820), Pal.Shade(sky, 0.95f), 70, 26);
    a.Streaks(new RectangleF(0, 0, 1920, 820), Pal.Shade(sky, 1.05f), 50, 20);

    // Slunce (stejné místo, kde ho kreslí hra: 1180, 380)
    if (clean)
    {
        for (int i = 0; i < 12; i++)
        {
            float t = MathF.Tau * i / 12;
            a.Stroke(Pal.Orange, 9, 1180 + MathF.Cos(t) * 112, 380 + MathF.Sin(t) * 112,
                1180 + MathF.Cos(t) * 150, 380 + MathF.Sin(t) * 150);
        }
        a.Shape(Ell(1180, 380, 90, 90), Pal.Yellow, 6, Pal.Orange);
        a.Shape(Ell(1152, 362, 8, 12), Pal.Black, 0);
        a.Shape(Ell(1208, 362, 8, 12), Pal.Black, 0);
        a.Stroke(Bezier(new(1140, 400), new(1160, 432), new(1200, 432), new(1220, 400), 16), Pal.Black, 6);
        a.Spray(1130, 404, 12, Pal.Orange, 60);
        a.Spray(1230, 404, 12, Pal.Orange, 60);

        // Mraky a ptáci
        foreach (var (x, y) in new[] { (250f, 470f), (1010f, 560f), (1600f, 230f) })
        {
            a.Shape(Ell(x, y, 70, 34), Pal.White, 4, Pal.LightGray);
            a.Shape(Ell(x + 55, y + 8, 46, 26), Pal.White, 4, Pal.LightGray);
            a.Shape(Ell(x - 50, y + 10, 40, 22), Pal.White, 4, Pal.LightGray);
            a.Shape(Ell(x, y + 10, 60, 24), Pal.White, 0);
        }
        foreach (var (x, y) in new[] { (420f, 330f), (470f, 360f), (900f, 420f), (1520f, 520f) })
            a.Stroke(Pal.Black, 3, x - 14, y - 6, x, y, x + 14, y - 6);
    }
    else
    {
        a.Shape(Ell(1180, 380, 90, 90), Pal.Hex("C2B072"), 5, Pal.Hex("8A7B4C"));
        a.Spray(1180, 380, 140, Pal.Hex("A39C8C"), 3000, 2);
        // Komíny v dálce
        foreach (var (x, h) in new[] { (140f, 150f), (330f, 110f), (1060f, 170f), (1250f, 120f), (1700f, 140f) })
        {
            a.Shape(Rect(x, 760 - h, 34, h + 20), Pal.Hex("5D554A"), 4, Pal.Hex("3E382F"));
            // Kouř stoupá a rozšiřuje se doprava nahoru
            for (int k = 0; k < 5; k++)
                a.Spray(x + 17 + k * 26, 745 - h - k * 34, 16 + k * 10, Pal.Hex("5E564B"), 220 + k * 120, 2);
        }
    }

    // Kopce v dálce (stejný tvar na obou obrázcích, ať se pěkně prolínají)
    var hills = new List<PointF> { new(0, 830) };
    for (int x = 0; x <= 1920; x += 40)
        hills.Add(new PointF(x, 742 - 38 * MathF.Sin(x / 260f) - 18 * MathF.Sin(x / 97f + 1)));
    hills.Add(new PointF(1920, 830));
    a.Shape(hills, clean ? Pal.Hex("8FD15F") : Pal.Hex("70685A"), 5, clean ? Pal.Hex("3D7A2A") : Pal.Hex("4E483E"));

    // Zem
    var ground = clean ? Pal.Hex("5CB85C") : Pal.Hex("6B5A45");
    a.Shape(Rect(-10, 820, 1940, 270), ground, 5, clean ? Pal.Hex("2E7D32") : Pal.Hex("3E3226"), amp: 2);
    a.Streaks(new RectangleF(0, 830, 1920, 250), Pal.Shade(ground, 0.9f), 60, 18);

    if (clean)
    {
        for (int i = 0; i < 260; i++)
        {
            float x = a.R.Next(0, 1920), y = a.R.Next(835, 1075);
            a.Stroke(Pal.Hex("2E8B3A"), 3, x, y, x + a.R.Next(-5, 6), y - a.R.Next(8, 18));
        }
        var flowers = new[] { Pal.Rose, Pal.Yellow, Pal.White, Pal.Purple };
        for (int i = 0; i < 70; i++)
            a.Shape(Ell(a.R.Next(0, 1920), a.R.Next(850, 1070), 5, 5), flowers[i % flowers.Length], 2);
    }
    else
    {
        // Praskliny, suchá tráva, pařez a pneumatika
        for (int i = 0; i < 18; i++)
        {
            float x = a.R.Next(0, 1900), y = a.R.Next(850, 1060);
            a.Stroke(Pal.Hex("3E3226"), 3, x, y, x + 20, y + 8, x + 34, y + 2, x + 52, y + 12);
        }
        for (int i = 0; i < 120; i++)
        {
            float x = a.R.Next(0, 1920), y = a.R.Next(835, 1075);
            a.Stroke(Pal.Hex("9C8B5A"), 3, x, y, x + a.R.Next(-7, 8), y - a.R.Next(6, 14));
        }
        a.Shape(Rect(860, 870, 50, 34), Pal.Hex("7A5236"), 4, Pal.Hex("3E3226"));
        a.Shape(Ell(885, 870, 25, 9), Pal.Hex("A07A55"), 4, Pal.Hex("3E3226"));
        a.Shape(Ell(1290, 1040, 34, 14), Pal.Hex("2B2B2B"), 4);
        a.Shape(Ell(1290, 1040, 16, 6), Pal.Hex("6B5A45"), 3);
    }
    a.Save(path);
}

// =====================================================================
//  KONCOVKA 1920×1080 (planeta vždy: střed 960, 560, poloměr 250)
// =====================================================================

const float PX = 960, PY = 560, PR = 250;

static Art Space(int seed, Color background)
{
    var a = new Art(1920, 1080, seed, background);
    a.Amp = 3;
    a.Stars(170, new Rectangle(0, 0, 1920, 970));
    return a;
}

static void Ending1(string path)
{
    using var a = Space(21, Pal.Hex("0B0D2A"));
    var top = Parts.Planet(a, PX, PY, PR);
    Parts.FlowerOnPlanet(a, top, 0.36f, Parts.Face.Smile);
    a.Text("O2: 100 %", 470, 130, 120, Pal.Lime, 10);
    a.Text("HURÁ!", 1500, 260, 90, Pal.Gold, 8);
    a.Text("Vzduch na Zemi je dokonale čistý...", 960, 900, 58, Pal.White, 6);
    a.Save(path);
}

static void Ending2(string path)
{
    using var a = Space(22, Pal.Hex("0B0D2A"));
    var top = Parts.Planet(a, PX, PY, PR);
    Parts.FlowerOnPlanet(a, top, 0.36f, Parts.Face.Scared);

    // Úniková raketka Čmoud a.s.
    a.Shape(Pts(1745, 520, 1900, 470, 1860, 560, 1900, 650, 1745, 600), Pal.Orange, 6);
    a.Shape(Pts(1745, 540, 1840, 520, 1815, 560, 1840, 600, 1745, 580), Pal.Yellow, 0);
    a.Shape(Ell(1560, 560, 200, 115), Pal.LightGray, 7);
    a.Spray(1450, 520, 50, Pal.Gray, 400, 2);
    a.Shape(Rect(1372, 598, 376, 36), Pal.Red, 0);
    a.Text("Čmoud a.s.", 1545, 615, 30, Pal.White);
    a.Shape(Ell(1610, 530, 78, 70), Pal.Hex("1D2433"), 7);
    Parts.VillainHead(a, 1610, 545, 0.62f);

    // Ruka se sirkami
    a.OutlinedStroke(Pts(1385, 540, 1330, 520, 1300, 520), Pal.LightGray, 22, 5);
    a.Shape(Ell(1295, 520, 22, 20), Pal.LightGray, 5);
    a.Shape(Rect(1215, 480, 78, 48), Pal.Red, 5);
    a.Text("SIRKY", 1254, 504, 20, Pal.White);

    // Bublina
    a.Shape(Pts(1490, 240, 1580, 250, 1600, 455), Pal.White, 6);
    a.Shape(Ell(1450, 160, 330, 105), Pal.White, 6);
    a.Shape(Pts(1500, 230, 1570, 236, 1580, 270), Pal.White, 0);
    a.Text("Když ne smog...", 1450, 125, 56, Pal.Black);
    a.Text("tak aspoň OHEŇ!", 1450, 195, 56, Pal.Red);
    a.Save(path);
}

static void Ending3(string path)
{
    using var a = Space(23, Pal.Hex("2A0F1A"));
    // Atmosféra začíná žhnout
    a.Shape(Ell(PX, PY, 330, 330), Pal.Orange, 0, amp: 8);
    a.Spray(PX, PY, 330, Pal.Yellow, 4000, 3);
    a.Spray(PX, PY, 340, Pal.Red, 1500, 3);
    var top = Parts.Planet(a, PX, PY, PR);
    Parts.FlowerOnPlanet(a, top, 0.36f, Parts.Face.Scared);

    // Robotí ruka se zapálenou sirkou
    a.Shape(Pts(1925, 1085, 1925, 870, 1530, 650, 1440, 770, 1700, 1085), Pal.LightGray, 7);
    a.Spray(1700, 860, 70, Pal.Gray, 350, 2);
    a.OutlinedStroke(Pts(1440, 660, 1300, 500), Pal.LightYellow, 14, 4);
    a.Shape(Ell(1470, 690, 85, 70), Pal.LightGray, 7);
    a.Stroke(Pal.Black, 5, 1420, 660, 1500, 650);
    a.Stroke(Pal.Black, 5, 1418, 690, 1505, 682);
    a.Stroke(Pal.Black, 5, 1425, 720, 1500, 715);
    a.Shape(Ell(1295, 492, 22, 26, 40), Pal.Red, 5);
    a.Shape(Pts(1300, 470, 1330, 410, 1300, 350, 1285, 300, 1255, 380, 1250, 440, 1270, 475), Pal.Orange, 6);
    a.Shape(Pts(1292, 462, 1305, 410, 1288, 360, 1270, 420, 1272, 455), Pal.Yellow, 0);
    a.Shape(Ell(1286, 440, 8, 16), Pal.White, 0);
    a.Text("ŠKRT!", 400, 170, 170, Pal.Yellow, 12);
    a.Save(path);
}

static void Ending4(string path)
{
    using var a = new Art(1920, 1080, 24, Pal.Orange);
    a.Amp = 4;
    a.Shape(a.Burst(PX, PY, 820, 560, 16, 0.25f), Pal.Red, 8);
    a.Shape(a.Burst(PX, PY, 560, 360, 14, 0.25f), Pal.Gold, 7);
    a.Shape(a.Burst(PX, PY, 330, 210, 12, 0.2f), Pal.Yellow, 6);
    a.Shape(Ell(PX, PY, 150, 130), Pal.White, 0, amp: 8);
    a.Spray(PX, PY, 600, Pal.Yellow, 3000, 3);

    // Kusy planety letí pryč
    var chunks = new[] { (150f, 140f), (1760f, 150f), (130f, 900f), (1790f, 920f), (420f, 80f), (1480f, 1010f), (60f, 520f), (1860f, 560f) };
    var colors = new[] { Pal.Brown, Pal.Green, Pal.Turquoise, Pal.Brown };
    for (int i = 0; i < chunks.Length; i++)
    {
        var (x, y) = chunks[i];
        float dx = PX - x, dy = PY - y, len = MathF.Sqrt(dx * dx + dy * dy);
        for (int k = -1; k <= 1; k++)
            a.Stroke(Pal.Black, 5, x + dx / len * 60 + k * 18 * dy / len, y + dy / len * 60 - k * 18 * dx / len,
                x + dx / len * 150 + k * 18 * dy / len, y + dy / len * 150 - k * 18 * dx / len);
        a.Shape(a.Rock(x, y, 50), colors[i % colors.Length], 6);
    }
    a.Text("BUUUUUM!", PX, PY, 210, Pal.DarkRed, 14);
    a.Save(path);
}

static void Ending5(string path)
{
    using var a = Space(25, Pal.Hex("050510"));
    var colors = new[] { Pal.Brown, Pal.Green, Pal.Turquoise, Pal.Gray };
    var rocks = new[] { (250f, 480f), (380f, 820f), (820f, 560f), (930f, 820f), (1120f, 700f), (1660f, 860f), (1750f, 360f), (200f, 230f), (1700f, 150f), (1110f, 470f), (60f, 700f) };
    for (int i = 0; i < rocks.Length; i++)
        a.Shape(a.Rock(rocks[i].Item1, rocks[i].Item2, 24 + (i * 7) % 26), colors[i % colors.Length], 5);

    a.Text("Vzduch je čistý. Až moc.", 960, 230, 84, Pal.White, 6);
    a.Text("KONEC", 960, 370, 140, Pal.Gold, 10);

    // Kytka přežila v květináči
    Parts.Flower(a, 600, 660, 0.42f, Parts.Face.Dizzy);
    a.Shape(Pts(530, 652, 670, 652, 652, 760, 548, 760), Pal.Brown, 6);
    a.Shape(Rect(520, 640, 160, 24), Pal.Hex("A0603A"), 6);

    // Utržená hlava záporáka
    Parts.VillainHead(a, 1390, 700, 0.75f, broken: true);
    a.Spray(1390, 770, 40, Pal.Yellow, 120, 3);
    a.Shape(Pts(1360, 560, 1420, 562, 1390, 620), Pal.White, 5);
    a.Shape(Ell(1420, 490, 280, 82), Pal.White, 5);
    a.Shape(Pts(1368, 556, 1414, 558, 1392, 590), Pal.White, 0);
    a.Text("Chyba 404:", 1420, 462, 44, Pal.Black);
    a.Text("planeta nenalezena.", 1420, 516, 44, Pal.Black);
    a.Save(path);
}
