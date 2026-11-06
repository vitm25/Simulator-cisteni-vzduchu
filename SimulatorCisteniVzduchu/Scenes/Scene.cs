using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SimulatorCisteniVzduchu.Scenes;

/// <summary>
/// Základ pro všechny obrazovky hry (menu, hra, koncová animace...).
/// </summary>
public abstract class Scene
{
    protected AirGame Game { get; }

    protected Scene(AirGame game)
    {
        Game = game;
    }

    /// <summary>Zavolá se jednou, když se scéna stane aktivní.</summary>
    public virtual void Load() { }

    /// <summary>Zavolá se, když scéna končí (uvolnění zdrojů).</summary>
    public virtual void Unload() { }

    public abstract void Update(GameTime gameTime);

    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
}
