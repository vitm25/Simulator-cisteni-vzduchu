using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SimulatorCisteniVzduchu.Scenes;

/// <summary>
/// Hlavní herní obrazovka. Zatím jen placeholder: šedivé nebe a zem.
/// Sem později přijde kytka na klikání, smog, upgrady a ukazatel O₂.
/// </summary>
public class GameScene : Scene
{
    private static readonly Color SmogSky = new(120, 120, 110);
    private static readonly Color Ground = new(90, 140, 60);

    public GameScene(AirGame game) : base(game) { }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        Game.GraphicsDevice.Clear(SmogSky);

        spriteBatch.Begin();
        int groundTop = AirGame.ScreenHeight * 3 / 4;
        spriteBatch.Draw(
            Game.Pixel,
            new Rectangle(0, groundTop, AirGame.ScreenWidth, AirGame.ScreenHeight - groundTop),
            Ground);
        spriteBatch.End();
    }
}
