using System.Drawing;

namespace Kreslirna;

/// <summary>Díly, které se kreslí na víc obrázků (kytka, hlava záporáka, planeta).</summary>
static class Parts
{
    public static readonly Color FlowerPink = Pal.Hex("FF6FA8");
    public static readonly Color StemGreen = Pal.Green;
    public static readonly Color Metal = Pal.LightGray;
    public static readonly Color DarkMetal = Pal.Hex("5C6068");

    public enum Face { Smile, Scared, Dizzy }

    /// <summary>
    /// Kytka. (baseX, baseY) = pata stonku, s = měřítko (1 = 300×530 px, hlava 390 px nad patou).
    /// </summary>
    public static void Flower(Art a, float baseX, float baseY, float s, Face face)
    {
        float hx = baseX, hy = baseY - 390 * s;

        // Stonek
        var stem = Art.Bezier(new PointF(baseX, baseY), new PointF(baseX - 30 * s, baseY - 140 * s),
            new PointF(baseX + 30 * s, baseY - 260 * s), new PointF(hx, hy));
        a.OutlinedStroke(stem, StemGreen, 13 * s, 3.5f * s);

        // Listy
        a.Shape(Art.Ell(baseX - 52 * s, baseY - 150 * s, 50 * s, 20 * s, -28), Pal.Green, 4 * s);
        a.Stroke(Art.Pts(baseX - 90 * s, baseY - 128 * s, baseX - 14 * s, baseY - 170 * s), Pal.Hex("17803A"), 3 * s);
        a.Shape(Art.Ell(baseX + 54 * s, baseY - 236 * s, 48 * s, 19 * s, 26), Pal.Green, 4 * s);
        a.Stroke(Art.Pts(baseX + 16 * s, baseY - 254 * s, baseX + 92 * s, baseY - 216 * s), Pal.Hex("17803A"), 3 * s);

        // Okvětní lístky
        for (int i = 0; i < 6; i++)
        {
            float t = MathF.Tau * i / 6 - MathF.PI / 2;
            a.Shape(Art.Ell(hx + MathF.Cos(t) * 72 * s, hy + MathF.Sin(t) * 72 * s, 50 * s, 50 * s),
                FlowerPink, 5 * s);
        }
        for (int i = 0; i < 6; i++)
        {
            float t = MathF.Tau * i / 6 - MathF.PI / 2;
            a.Spray(hx + MathF.Cos(t) * 82 * s, hy + MathF.Sin(t) * 82 * s, 24 * s, Pal.Hex("E0508A"), (int)(60 * s * s) + 4);
        }

        // Střed s obličejem
        a.Shape(Art.Ell(hx, hy, 52 * s, 52 * s), Pal.Gold, 5 * s);
        switch (face)
        {
            case Face.Smile:
                a.Shape(Art.Ell(hx - 18 * s, hy - 10 * s, 6 * s, 10 * s), Pal.Black, 0);
                a.Shape(Art.Ell(hx + 18 * s, hy - 10 * s, 6 * s, 10 * s), Pal.Black, 0);
                a.Stroke(Art.Bezier(new PointF(hx - 24 * s, hy + 12 * s), new PointF(hx - 12 * s, hy + 30 * s),
                    new PointF(hx + 12 * s, hy + 30 * s), new PointF(hx + 24 * s, hy + 12 * s), 12), Pal.Black, 4.5f * s);
                a.Spray(hx - 34 * s, hy + 12 * s, 9 * s, Pal.Red, (int)(30 * s) + 6);
                a.Spray(hx + 34 * s, hy + 12 * s, 9 * s, Pal.Red, (int)(30 * s) + 6);
                break;

            case Face.Scared:
                a.Shape(Art.Ell(hx - 18 * s, hy - 12 * s, 13 * s, 15 * s), Pal.White, 3 * s);
                a.Shape(Art.Ell(hx + 18 * s, hy - 12 * s, 13 * s, 15 * s), Pal.White, 3 * s);
                a.Shape(Art.Ell(hx - 18 * s, hy - 10 * s, 4 * s, 4 * s), Pal.Black, 0);
                a.Shape(Art.Ell(hx + 18 * s, hy - 10 * s, 4 * s, 4 * s), Pal.Black, 0);
                a.Shape(Art.Ell(hx, hy + 22 * s, 11 * s, 14 * s), Pal.DarkRed, 4 * s);
                // kapka potu
                a.Shape(Art.Pts(hx + 46 * s, hy - 46 * s, hx + 54 * s, hy - 26 * s, hx + 46 * s, hy - 18 * s,
                    hx + 38 * s, hy - 26 * s), Pal.LightTurquoise, 3 * s);
                break;

            case Face.Dizzy:
                foreach (float ex in new[] { -18f, 18f })
                {
                    a.Stroke(Pal.Black, 4 * s, hx + (ex - 8) * s, hy - 18 * s, hx + (ex + 8) * s, hy - 2 * s);
                    a.Stroke(Pal.Black, 4 * s, hx + (ex + 8) * s, hy - 18 * s, hx + (ex - 8) * s, hy - 2 * s);
                }
                a.Stroke(Art.Pts(hx - 22 * s, hy + 22 * s, hx - 11 * s, hy + 15 * s, hx, hy + 22 * s,
                    hx + 11 * s, hy + 15 * s, hx + 22 * s, hy + 22 * s), Pal.Black, 4 * s);
                break;
        }
    }

    /// <summary>Hlava SMOGOTRONa 3000. (cx, cy) = střed hlavy, s = měřítko (1 = hlava 154×126 px).</summary>
    public static void VillainHead(Art a, float cx, float cy, float s, bool broken = false)
    {
        float L = cx - 77 * s, T = cy - 63 * s;

        // Anténa
        if (broken)
        {
            a.Stroke(Pal.Black, 5 * s, cx, T, cx + 6 * s, T - 22 * s, cx + 30 * s, T - 30 * s);
            a.Shape(Art.Ell(cx + 34 * s, T - 32 * s, 8 * s, 8 * s), Pal.DarkRed, 3 * s);
        }
        else
        {
            a.Stroke(Pal.Black, 5 * s, cx, T, cx, T - 30 * s);
            a.Shape(Art.Ell(cx, T - 36 * s, 10 * s, 10 * s), Pal.Red, 3.5f * s);
            a.Spray(cx, T - 36 * s, 22 * s, Pal.Hex("FF8080"), (int)(40 * s) + 4);
        }

        // Hlava
        a.Shape(Art.Rect(L, T, 154 * s, 126 * s), Metal, 5 * s);
        a.Spray(L + 120 * s, T + 100 * s, 30 * s, Pal.Gray, (int)(120 * s) + 6);
        foreach (var (rx, ry) in new[] { (10f, 10f), (144f, 10f), (10f, 116f), (144f, 116f) })
            a.Shape(Art.Ell(L + rx * s, T + ry * s, 4 * s, 4 * s), DarkMetal, 0);

        // Oči a obočí
        Color eye = broken ? Pal.DarkRed : Pal.Red;
        a.Shape(Art.Rect(L + 22 * s, T + 40 * s, 44 * s, 26 * s), eye, 4 * s);
        a.Shape(Art.Rect(L + 88 * s, T + 40 * s, 44 * s, 26 * s), eye, 4 * s);
        if (!broken)
        {
            a.Shape(Art.Rect(L + 30 * s, T + 45 * s, 9 * s, 7 * s), Pal.Yellow, 0);
            a.Shape(Art.Rect(L + 96 * s, T + 45 * s, 9 * s, 7 * s), Pal.Yellow, 0);
            a.Stroke(Pal.Black, 9 * s, L + 18 * s, T + 22 * s, L + 70 * s, T + 36 * s);
            a.Stroke(Pal.Black, 9 * s, L + 136 * s, T + 22 * s, L + 84 * s, T + 36 * s);
        }
        else
        {
            a.Stroke(Pal.Black, 4 * s, L + 22 * s, T + 40 * s, L + 66 * s, T + 66 * s);
            a.Stroke(Pal.Black, 4 * s, L + 132 * s, T + 40 * s, L + 88 * s, T + 66 * s);
            // prasklina
            a.Stroke(Pal.Black, 3 * s, L + 110 * s, T, L + 100 * s, T + 18 * s, L + 116 * s, T + 30 * s, L + 106 * s, T + 40 * s);
        }

        // Mřížka úst
        a.Shape(Art.Rect(L + 38 * s, T + 84 * s, 78 * s, 26 * s), DarkMetal, 4 * s);
        for (int i = 1; i < 5; i++)
            a.Stroke(Metal, 3 * s, L + (38 + i * 15.6f) * s, T + 87 * s, L + (38 + i * 15.6f) * s, T + 107 * s);
    }

    /// <summary>Planeta Země. Vrací souřadnice „vršku“ planety (kam se zapíchne kytka).</summary>
    public static PointF Planet(Art a, float cx, float cy, float r)
    {
        a.Shape(Art.Ell(cx, cy, r, r), Pal.Turquoise, 8, amp: 3);
        a.Spray(cx + r * 0.4f, cy + r * 0.4f, r * 0.35f, Pal.Hex("0080C0"), 900);

        float oldAmp = a.Amp;
        a.Amp = 7;
        a.Shape(Art.Ell(cx - r * 0.38f, cy - r * 0.22f, r * 0.3f, r * 0.24f, 20), Pal.Green, 5);
        a.Shape(Art.Ell(cx + r * 0.28f, cy + r * 0.12f, r * 0.24f, r * 0.34f, -15), Pal.Green, 5);
        a.Shape(Art.Ell(cx - r * 0.18f, cy + r * 0.52f, r * 0.26f, r * 0.12f, 5), Pal.Green, 5);
        a.Shape(Art.Ell(cx + r * 0.48f, cy - r * 0.48f, r * 0.12f, r * 0.08f), Pal.Lime, 4);
        a.Amp = oldAmp;
        a.Spray(cx - r * 0.38f, cy - r * 0.2f, r * 0.2f, Pal.Hex("17803A"), 250);
        a.Spray(cx + r * 0.28f, cy + r * 0.15f, r * 0.2f, Pal.Hex("17803A"), 250);

        // Mraky a odlesk
        a.Stroke(Art.Bezier(new(cx - r * 0.7f, cy + r * 0.05f), new(cx - r * 0.5f, cy - r * 0.05f),
            new(cx - r * 0.3f, cy + r * 0.12f), new(cx - r * 0.05f, cy + r * 0.02f), 14), Pal.White, 12);
        a.Stroke(Art.Bezier(new(cx + r * 0.05f, cy - r * 0.62f), new(cx + r * 0.2f, cy - r * 0.7f),
            new(cx + r * 0.35f, cy - r * 0.58f), new(cx + r * 0.52f, cy - r * 0.64f), 14), Pal.White, 10);
        a.Stroke(Art.Bezier(new(cx - r * 0.72f, cy - r * 0.32f), new(cx - r * 0.66f, cy - r * 0.55f),
            new(cx - r * 0.5f, cy - r * 0.7f), new(cx - r * 0.3f, cy - r * 0.78f), 14), Pal.White, 9);

        return new PointF(cx, cy - r);
    }

    /// <summary>Kytka zapíchnutá do vršku planety, s hromádkou hlíny.</summary>
    public static void FlowerOnPlanet(Art a, PointF top, float s, Face face)
    {
        Flower(a, top.X, top.Y + 14, s, face);
        a.Shape(Art.Ell(top.X, top.Y + 6, 34, 12), Pal.Brown, 4);
    }
}
