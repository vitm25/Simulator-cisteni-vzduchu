using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SimulatorCisteniVzduchu.UI;

/// <summary>Pomocné kreslicí metody, ať se v scénách nemusí pořád opakovat to samé.</summary>
public static class DrawExtensions
{
    public static void DrawRect(this SpriteBatch batch, Rectangle rect, Color color) =>
        batch.Draw(Assets.Pixel, rect, color);

    /// <summary>Obrys obdélníku dané tloušťky.</summary>
    public static void DrawFrame(this SpriteBatch batch, Rectangle rect, int thickness, Color color)
    {
        batch.DrawRect(new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
        batch.DrawRect(new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness), color);
        batch.DrawRect(new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        batch.DrawRect(new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height), color);
    }

    public static void DrawCircle(this SpriteBatch batch, Vector2 center, float radius, Color color)
    {
        var size = new Vector2(radius * 2);
        batch.Draw(Assets.Circle, new Rectangle((center - size / 2).ToPoint(), size.ToPoint()), color);
    }

    /// <summary>Text se stínem (lépe se čte na pestrém pozadí).</summary>
    public static void DrawText(this SpriteBatch batch, SpriteFont font, string text, Vector2 position,
        Color color, float scale = 1f)
    {
        batch.DrawString(font, text, position + new Vector2(3, 3), Color.Black * 0.45f * (color.A / 255f),
            0, Vector2.Zero, scale, SpriteEffects.None, 0);
        batch.DrawString(font, text, position, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
    }

    /// <summary>Zalomí text do řádků tak, aby se vešel do šířky <paramref name="maxWidth"/>.</summary>
    public static string WrapText(SpriteFont font, string text, float maxWidth)
    {
        var result = new System.Text.StringBuilder();
        string line = "";
        foreach (string word in text.Split(' '))
        {
            string candidate = line.Length == 0 ? word : line + " " + word;
            if (font.MeasureString(candidate).X > maxWidth && line.Length > 0)
            {
                result.Append(line).Append('\n');
                line = word;
            }
            else
            {
                line = candidate;
            }
        }
        result.Append(line);
        return result.ToString();
    }

    /// <summary>Text vycentrovaný na daný bod.</summary>
    public static void DrawTextCentered(this SpriteBatch batch, SpriteFont font, string text, Vector2 center,
        Color color, float scale = 1f)
    {
        Vector2 size = font.MeasureString(text) * scale;
        batch.DrawText(font, text, center - size / 2, color, scale);
    }
}
