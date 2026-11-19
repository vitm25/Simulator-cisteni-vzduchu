using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SimulatorCisteniVzduchu.UI;

/// <summary>
/// Obrázky krajiny a vylepšení z Content/Sprites. Každý je nepovinný –
/// když chybí (null), scéna místo něj nakreslí jednoduchý tvar.
/// </summary>
public static class SceneryArt
{
    public static Texture2D SkySmog { get; private set; }
    public static Texture2D SkyClean { get; private set; }
    public static Texture2D SmogCloud { get; private set; }
    public static Texture2D Flower { get; private set; }
    public static Texture2D Tree { get; private set; }
    public static Texture2D Greenhouse { get; private set; }
    public static Texture2D Field { get; private set; }
    public static Texture2D Robot { get; private set; }
    public static Texture2D Drone { get; private set; }
    public static Texture2D Bee { get; private set; }

    public static void Load(ContentManager content)
    {
        SkySmog = Assets.TryLoadTexture(content, "Sprites/bg_sky_smog");
        SkyClean = Assets.TryLoadTexture(content, "Sprites/bg_sky_clean");
        SmogCloud = Assets.TryLoadTexture(content, "Sprites/smog_cloud");
        Flower = Assets.TryLoadTexture(content, "Sprites/flower");
        Tree = Assets.TryLoadTexture(content, "Sprites/tree");
        Greenhouse = Assets.TryLoadTexture(content, "Sprites/greenhouse");
        Field = Assets.TryLoadTexture(content, "Sprites/field");
        Robot = Assets.TryLoadTexture(content, "Sprites/robot");
        Drone = Assets.TryLoadTexture(content, "Sprites/drone");
        Bee = Assets.TryLoadTexture(content, "Sprites/bee");
    }
}
