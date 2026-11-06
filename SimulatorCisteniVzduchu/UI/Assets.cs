using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SimulatorCisteniVzduchu.UI;

/// <summary>
/// Sdílené textury a fonty. Dokud nemáme grafiku z Malování, kreslí se všechno
/// z bílého pixelu a bílého kolečka obarvených na potřebnou barvu.
/// </summary>
public static class Assets
{
    public static Texture2D Pixel { get; private set; }
    public static Texture2D Circle { get; private set; }

    public static SpriteFont FontSmall { get; private set; }
    public static SpriteFont FontMain { get; private set; }
    public static SpriteFont FontBig { get; private set; }

    public static void Load(GraphicsDevice device, ContentManager content)
    {
        Pixel = new Texture2D(device, 1, 1);
        Pixel.SetData(new[] { Color.White });
        Circle = CreateCircle(device, 128);

        FontSmall = content.Load<SpriteFont>("Fonts/Small");
        FontMain = content.Load<SpriteFont>("Fonts/Main");
        FontBig = content.Load<SpriteFont>("Fonts/Big");
    }

    /// <summary>
    /// Zkusí načíst obrázek z Content. Když ještě neexistuje (není nakreslený), vrátí null
    /// a scéna místo něj nakreslí placeholder.
    /// </summary>
    public static Texture2D TryLoadTexture(ContentManager content, string name)
    {
        try
        {
            return content.Load<Texture2D>(name);
        }
        catch (ContentLoadException)
        {
            return null;
        }
    }

    /// <summary>Bílé kolečko s vyhlazeným okrajem.</summary>
    private static Texture2D CreateCircle(GraphicsDevice device, int size)
    {
        var data = new Color[size * size];
        float radius = size / 2f;
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dx = x + 0.5f - radius;
            float dy = y + 0.5f - radius;
            float distance = MathF.Sqrt(dx * dx + dy * dy);
            float alpha = Math.Clamp(radius - distance, 0, 1);
            data[y * size + x] = Color.White * alpha;
        }

        var texture = new Texture2D(device, size, size);
        texture.SetData(data);
        return texture;
    }
}
