using System;
using System.Diagnostics;
using System.IO;
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

    private static GraphicsDevice _device;

    public static void Load(GraphicsDevice device, ContentManager content)
    {
        _device = device;
        Pixel = new Texture2D(device, 1, 1);
        Pixel.SetData(new[] { Color.White });
        Circle = CreateCircle(device, 128);

        FontSmall = content.Load<SpriteFont>("Fonts/Small");
        FontMain = content.Load<SpriteFont>("Fonts/Main");
        FontBig = content.Load<SpriteFont>("Fonts/Big");
    }

    /// <summary>
    /// Zkusí načíst obrázek, např. "Sprites/factory" = soubor Content/Sprites/factory.png.
    /// PNG stačí uložit do složky, v MGCB Editoru se nic přidávat nemusí.
    /// Sytě růžová (#FF00FF) se změní na průhlednou.
    /// Když obrázek ještě neexistuje (není nakreslený), vrátí null
    /// a scéna místo něj nakreslí placeholder.
    /// </summary>
    public static Texture2D TryLoadTexture(ContentManager content, string name)
    {
        string basePath = Path.Combine(AppContext.BaseDirectory, content.RootDirectory,
            name.Replace('/', Path.DirectorySeparatorChar));

        // 1) Obyčejný PNG soubor (doporučeno).
        string png = basePath + ".png";
        if (File.Exists(png) && _device != null)
        {
            try
            {
                var texture = Texture2D.FromFile(_device, png);
                PrepareForDrawing(texture);
                return texture;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[Assets] Nejde načíst {png}: {e.Message}");
            }
        }

        // 2) Obrázek přidaný postaru přes MGCB Editor (.xnb).
        if (File.Exists(basePath + ".xnb"))
        {
            try
            {
                return content.Load<Texture2D>(name);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[Assets] Nejde načíst {name}.xnb: {e.Message}");
            }
        }

        return null;
    }

    /// <summary>
    /// Růžovou (#FF00FF) udělá průhlednou a barvy předpřipraví tak, jak je SpriteBatch
    /// očekává (premultiplied alpha) – jinak by průhledné okraje měly tmavé nebo barevné lemy.
    /// </summary>
    private static void PrepareForDrawing(Texture2D texture)
    {
        var data = new Color[texture.Width * texture.Height];
        texture.GetData(data);

        for (int i = 0; i < data.Length; i++)
        {
            var c = data[i];
            if (c.R >= 230 && c.G <= 30 && c.B >= 230)
            {
                data[i] = Color.Transparent;
            }
            else if (c.A < 255)
            {
                data[i] = new Color(c.R * c.A / 255, c.G * c.A / 255, c.B * c.A / 255, c.A);
            }
        }

        texture.SetData(data);
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
