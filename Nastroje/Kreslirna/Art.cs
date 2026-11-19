using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace Kreslirna;

/// <summary>Barvy z palety MS Malování (Windows 7/10/11).</summary>
static class Pal
{
    public static readonly Color Black = Hex("000000");
    public static readonly Color Gray = Hex("7F7F7F");
    public static readonly Color DarkRed = Hex("880015");
    public static readonly Color Red = Hex("ED1C24");
    public static readonly Color Orange = Hex("FF7F27");
    public static readonly Color Yellow = Hex("FFF200");
    public static readonly Color Green = Hex("22B14C");
    public static readonly Color Turquoise = Hex("00A2E8");
    public static readonly Color Indigo = Hex("3F48CC");
    public static readonly Color Purple = Hex("A349A4");
    public static readonly Color White = Hex("FFFFFF");
    public static readonly Color LightGray = Hex("C3C3C3");
    public static readonly Color Brown = Hex("B97A57");
    public static readonly Color Rose = Hex("FFAEC9");
    public static readonly Color Gold = Hex("FFC90E");
    public static readonly Color LightYellow = Hex("EFE4B0");
    public static readonly Color Lime = Hex("B5E61D");
    public static readonly Color LightTurquoise = Hex("99D9EA");
    public static readonly Color BlueGray = Hex("7092BE");
    public static readonly Color Lavender = Hex("C8BFE7");

    public static Color Hex(string hex) => Color.FromArgb(
        Convert.ToInt32(hex[..2], 16), Convert.ToInt32(hex[2..4], 16), Convert.ToInt32(hex[4..6], 16));

    /// <summary>Ztmaví (factor &lt; 1) nebo zesvětlí (factor &gt; 1) barvu.</summary>
    public static Color Shade(Color c, float factor) => Color.FromArgb(c.A,
        Math.Clamp((int)(c.R * factor), 0, 255),
        Math.Clamp((int)(c.G * factor), 0, 255),
        Math.Clamp((int)(c.B * factor), 0, 255));
}

/// <summary>
/// Plátno, na které se „kreslí rukou v Malování“: bez vyhlazování, čáry se lehce třesou,
/// výplně jsou plné barvy jako z kyblíku.
/// </summary>
sealed class Art : IDisposable
{
    public readonly Bitmap Bmp;
    public readonly Graphics G;
    public readonly Random R;

    /// <summary>Jak moc se čáry třesou (v pixelech).</summary>
    public float Amp = 1.5f;

    public int W => Bmp.Width;
    public int H => Bmp.Height;

    public Art(int width, int height, int seed, Color? background = null)
    {
        Bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        G = Graphics.FromImage(Bmp);
        G.SmoothingMode = SmoothingMode.None;
        G.InterpolationMode = InterpolationMode.NearestNeighbor;
        G.PixelOffsetMode = PixelOffsetMode.Half;
        G.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
        G.Clear(background ?? Color.Transparent);
        R = new Random(seed);
    }

    public void Save(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        Bmp.Save(path, ImageFormat.Png);
        Console.WriteLine($"  {Path.GetFileName(path),-24} {W}×{H}");
    }

    public void Dispose()
    {
        G.Dispose();
        Bmp.Dispose();
    }

    // ---------------------------------------------------------------- body a tvary

    public static PointF[] Pts(params float[] xy)
    {
        var result = new PointF[xy.Length / 2];
        for (int i = 0; i < result.Length; i++)
            result[i] = new PointF(xy[i * 2], xy[i * 2 + 1]);
        return result;
    }

    public static List<PointF> Ell(float cx, float cy, float rx, float ry, float rotDeg = 0, int n = 0)
    {
        if (n == 0)
            n = Math.Clamp((int)((rx + ry) * 0.5f), 10, 120);
        float rot = rotDeg * MathF.PI / 180;
        float cos = MathF.Cos(rot), sin = MathF.Sin(rot);
        var list = new List<PointF>(n);
        for (int i = 0; i < n; i++)
        {
            float t = MathF.Tau * i / n;
            float x = rx * MathF.Cos(t), y = ry * MathF.Sin(t);
            list.Add(new PointF(cx + x * cos - y * sin, cy + x * sin + y * cos));
        }
        return list;
    }

    public static PointF[] Rect(float x, float y, float w, float h) => Pts(x, y, x + w, y, x + w, y + h, x, y + h);

    public static List<PointF> Bezier(PointF p0, PointF p1, PointF p2, PointF p3, int n = 30)
    {
        var list = new List<PointF>(n + 1);
        for (int i = 0; i <= n; i++)
        {
            float t = (float)i / n, u = 1 - t;
            float a = u * u * u, b = 3 * u * u * t, c = 3 * u * t * t, d = t * t * t;
            list.Add(new PointF(
                a * p0.X + b * p1.X + c * p2.X + d * p3.X,
                a * p0.Y + b * p1.Y + c * p2.Y + d * p3.Y));
        }
        return list;
    }

    /// <summary>Hvězdicový „výbuch“ s nepravidelnými cípy.</summary>
    public List<PointF> Burst(float cx, float cy, float outer, float inner, int spikes, float jitter)
    {
        var list = new List<PointF>();
        float start = (float)(R.NextDouble() * MathF.Tau);
        for (int i = 0; i < spikes * 2; i++)
        {
            float t = start + MathF.PI * i / spikes + (float)(R.NextDouble() - 0.5) * 0.12f;
            float r = (i % 2 == 0 ? outer : inner) * (1 + (float)(R.NextDouble() - 0.5) * jitter);
            list.Add(new PointF(cx + MathF.Cos(t) * r, cy + MathF.Sin(t) * r));
        }
        return list;
    }

    /// <summary>Nepravidelný kámen / kus planety.</summary>
    public List<PointF> Rock(float cx, float cy, float size)
    {
        int n = R.Next(5, 8);
        var list = new List<PointF>();
        float start = (float)(R.NextDouble() * MathF.Tau);
        for (int i = 0; i < n; i++)
        {
            float t = start + MathF.Tau * i / n;
            float r = size * (0.6f + (float)R.NextDouble() * 0.5f);
            list.Add(new PointF(cx + MathF.Cos(t) * r, cy + MathF.Sin(t) * r));
        }
        return list;
    }

    // ---------------------------------------------------------------- třes ruky

    private List<PointF> Wobble(IReadOnlyList<PointF> pts, bool closed, float amp)
    {
        var result = new List<PointF>();
        if (amp <= 0 || pts.Count < 2)
        {
            result.AddRange(pts);
            return result;
        }

        float ph1 = (float)(R.NextDouble() * 50), ph2 = (float)(R.NextDouble() * 50);
        float distance = 0;
        int n = pts.Count;
        int segments = closed ? n : n - 1;
        for (int i = 0; i < segments; i++)
        {
            var a = pts[i];
            var b = pts[(i + 1) % n];
            float dx = b.X - a.X, dy = b.Y - a.Y;
            float len = MathF.Sqrt(dx * dx + dy * dy);
            if (len < 0.01f)
                continue;
            float nx = -dy / len, ny = dx / len;
            int steps = Math.Max(1, (int)(len / 7));
            for (int k = 0; k < steps; k++)
            {
                float t = (float)k / steps;
                float d = distance + len * t;
                float off = amp * (0.65f * MathF.Sin(d / 23f + ph1) + 0.35f * MathF.Sin(d / 7.7f + ph2));
                result.Add(new PointF(a.X + dx * t + nx * off, a.Y + dy * t + ny * off));
            }
            distance += len;
        }
        if (!closed)
            result.Add(pts[n - 1]);
        return result;
    }

    private static Pen MakePen(Color color, float width) => new(color, width)
    {
        LineJoin = LineJoin.Round,
        StartCap = LineCap.Round,
        EndCap = LineCap.Round,
    };

    // ---------------------------------------------------------------- kreslení

    /// <summary>Uzavřený tvar: výplň kyblíkem + roztřesený obrys (outline = 0 → bez obrysu).</summary>
    public void Shape(IReadOnlyList<PointF> pts, Color? fill, float outline = 4, Color? lineColor = null, float amp = -1)
    {
        var wobbled = Wobble(pts, true, amp < 0 ? Amp : amp).ToArray();
        if (wobbled.Length < 3)
            return;
        if (fill is Color f)
            using (var brush = new SolidBrush(f))
                G.FillPolygon(brush, wobbled);
        if (outline > 0)
            using (var pen = MakePen(lineColor ?? Pal.Black, outline))
                G.DrawPolygon(pen, wobbled);
    }

    /// <summary>Otevřená čára štětcem.</summary>
    public void Stroke(IReadOnlyList<PointF> pts, Color color, float width, float amp = -1)
    {
        var wobbled = Wobble(pts, false, amp < 0 ? Amp : amp).ToArray();
        if (wobbled.Length < 2)
            return;
        using var pen = MakePen(color, width);
        G.DrawLines(pen, wobbled);
    }

    public void Stroke(Color color, float width, params float[] xy) => Stroke(Pts(xy), color, width);

    /// <summary>Čára s černým obrysem (jako dvakrát přetažená štětcem).</summary>
    public void OutlinedStroke(IReadOnlyList<PointF> pts, Color color, float width, float outline)
    {
        var wobbled = Wobble(pts, false, Amp);
        Stroke(wobbled, Pal.Black, width + outline * 2, 0);
        Stroke(wobbled, color, width, 0);
    }

    /// <summary>Sprej z Malování: náhodné tečky v kruhu.</summary>
    public void Spray(float cx, float cy, float radius, Color color, int count, int dot = 1)
    {
        using var brush = new SolidBrush(color);
        for (int i = 0; i < count; i++)
        {
            float t = (float)(R.NextDouble() * MathF.Tau);
            float r = radius * MathF.Sqrt((float)R.NextDouble());
            G.FillRectangle(brush, (int)(cx + MathF.Cos(t) * r), (int)(cy + MathF.Sin(t) * r), dot, dot);
        }
    }

    /// <summary>Plochu „vybarvenou fixou“ – vodorovné tahy štětcem v trochu jiném odstínu.</summary>
    public void Streaks(RectangleF area, Color color, int count, float width)
    {
        for (int i = 0; i < count; i++)
        {
            float y = area.Top + (float)R.NextDouble() * area.Height;
            float x = area.Left + (float)R.NextDouble() * area.Width;
            float len = 80 + (float)R.NextDouble() * 260;
            Stroke(Pts(x, y, x + len, y + (float)(R.NextDouble() - 0.5) * 8), color, width, 2);
        }
    }

    public void Stars(int count, Rectangle area)
    {
        using var white = new SolidBrush(Pal.White);
        using var yellow = new SolidBrush(Pal.LightYellow);
        for (int i = 0; i < count; i++)
        {
            int x = area.Left + R.Next(area.Width), y = area.Top + R.Next(area.Height);
            if (i % 9 == 0)
            {
                // větší hvězdička ve tvaru +
                G.FillRectangle(yellow, x - 6, y - 1, 13, 3);
                G.FillRectangle(yellow, x - 1, y - 6, 3, 13);
            }
            else
            {
                int s = R.Next(2, 4);
                G.FillRectangle(white, x, y, s, s);
            }
        }
    }

    /// <summary>Text nástrojem Text v Malování (Comic Sans), volitelně s obrysem.</summary>
    public void Text(string text, float x, float y, float size, Color fill,
        float outline = 0, Color? outlineColor = null, bool center = true, bool bold = true)
    {
        using var path = new GraphicsPath();
        using var family = new FontFamily("Comic Sans MS");
        using var format = new StringFormat
        {
            Alignment = center ? StringAlignment.Center : StringAlignment.Near,
            LineAlignment = StringAlignment.Center,
        };
        path.AddString(text, family, (int)(bold ? FontStyle.Bold : FontStyle.Regular), size, new PointF(x, y), format);
        if (outline > 0)
            using (var pen = MakePen(outlineColor ?? Pal.Black, outline))
                G.DrawPath(pen, path);
        using var brush = new SolidBrush(fill);
        G.FillPath(brush, path);
    }
}
