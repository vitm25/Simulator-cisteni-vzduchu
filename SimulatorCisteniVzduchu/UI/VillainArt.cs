using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SimulatorCisteniVzduchu.UI;

/// <summary>
/// Kreslení záporáka SMOGOTRONA 3000 a jeho továren. Pokud existují obrázky
/// z Malování (Sprites/villain_portrait, Sprites/factory), použijí se,
/// jinak se nakreslí placeholder z obdélníků a koleček.
/// </summary>
public static class VillainArt
{
    private static readonly Color Metal = new(125, 125, 135);
    private static readonly Color DarkMetal = new(60, 60, 70);
    private static readonly Color EvilRed = new(255, 40, 40);

    public static Texture2D Portrait { get; private set; }
    public static Texture2D FactoryTexture { get; private set; }

    public static void Load(Microsoft.Xna.Framework.Content.ContentManager content)
    {
        Portrait = Assets.TryLoadTexture(content, "Sprites/villain_portrait");
        FactoryTexture = Assets.TryLoadTexture(content, "Sprites/factory");
    }

    /// <summary>Hlava záporáka vepsaná do obdélníku <paramref name="area"/>.</summary>
    public static void DrawHead(SpriteBatch batch, Rectangle area, float time = 0, float alpha = 1f)
    {
        if (Portrait != null)
        {
            batch.Draw(Portrait, area, Color.White * alpha);
            return;
        }

        // Placeholder: hranatá robotí hlava s anténou a rudýma očima.
        float u = area.Width / 240f; // měřítko (původní návrh má 240 px)
        Vector2 P(float x, float y) => new(area.X + x * u, area.Y + y * u);
        Rectangle R(float x, float y, float w, float h) => new((int)P(x, y).X, (int)P(x, y).Y, (int)(w * u), (int)(h * u));
        Color metal = Metal * alpha, dark = DarkMetal * alpha, red = EvilRed * alpha;

        batch.DrawRect(R(115, 0, 10, 50), dark);                               // anténa
        float blink = 0.6f + 0.4f * MathF.Sin(time * 5);
        batch.DrawCircle(P(120, 0), 12 * u, red * blink);

        var head = R(0, 45, 240, 195);
        batch.DrawRect(head, metal);
        batch.DrawFrame(head, Math.Max(2, (int)(6 * u)), dark);

        batch.DrawRect(R(30, 95, 70, 12), dark);                                // zlé obočí
        batch.DrawRect(R(140, 95, 70, 12), dark);
        batch.DrawCircle(P(70, 130), 22 * u, red);                              // oči
        batch.DrawCircle(P(170, 130), 22 * u, red);
        batch.DrawCircle(P(70, 130), 8 * u, new Color(255, 200, 200) * alpha);
        batch.DrawCircle(P(170, 130), 8 * u, new Color(255, 200, 200) * alpha);

        var mouth = R(50, 185, 140, 26);                                         // mřížka úst
        batch.DrawRect(mouth, dark);
        for (int i = 1; i < 5; i++)
            batch.DrawRect(R(50 + i * 28, 185, 4, 26), metal);
    }

    /// <summary>Továrna. <paramref name="build"/> 0..1 = jak moc už vyrostla ze země.</summary>
    public static void DrawFactory(SpriteBatch batch, Rectangle bounds, float time, float build, int seed)
    {
        // Vyrůstání: ořízneme výšku podle postupu stavby.
        int visibleHeight = (int)(bounds.Height * build);
        var visible = new Rectangle(bounds.X, bounds.Bottom - visibleHeight, bounds.Width, visibleHeight);

        if (FactoryTexture != null)
        {
            var source = new Rectangle(0, 0, FactoryTexture.Width, (int)(FactoryTexture.Height * build));
            batch.Draw(FactoryTexture, visible, source, Color.White);
        }
        else
        {
            // Placeholder: hala se zubatou střechou a komínem.
            int top = bounds.Bottom - (int)(140 * build);
            var hall = new Rectangle(bounds.X, top, bounds.Width, bounds.Bottom - top);
            batch.DrawRect(hall, new Color(105, 85, 75));
            batch.DrawFrame(hall, 3, new Color(55, 45, 40));
            for (int i = 0; i < 3 && build >= 1; i++)
            {
                batch.DrawRect(new Rectangle(bounds.X + 18 + i * 52, top + 45, 30, 30), new Color(255, 190, 60));
                batch.DrawRect(new Rectangle(bounds.X + i * 57, top - 20, 40, 20), new Color(85, 70, 60));
            }

            int chimneyTop = bounds.Bottom - visibleHeight;
            batch.DrawRect(new Rectangle(bounds.Right - 55, chimneyTop, 34, bounds.Bottom - chimneyTop), new Color(90, 75, 65));
            batch.DrawRect(new Rectangle(bounds.Right - 55, chimneyTop, 34, 10), EvilRed * 0.8f);
            batch.DrawText(Assets.FontSmall, "Čmoud a.s.", new Vector2(bounds.X + 12, bounds.Bottom - 40), Color.White * 0.85f);
        }

        if (build < 1)
            return;

        // Kouř z komína
        var chimney = new Vector2(bounds.Right - 38, bounds.Y);
        for (int k = 0; k < 6; k++)
        {
            float phase = (time * 0.45f + k / 6f + seed * 0.13f) % 1f;
            var pos = chimney + new Vector2(phase * 60 + MathF.Sin(time + k) * 8, -phase * 220);
            batch.DrawCircle(pos, 16 + phase * 34, new Color(60, 55, 50) * (0.75f * (1 - phase)));
        }
    }
}
