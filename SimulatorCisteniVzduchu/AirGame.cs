using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimulatorCisteniVzduchu.Scenes;

namespace SimulatorCisteniVzduchu;

/// <summary>
/// Vstupní třída hry. Nastaví okno a předává Update/Draw aktuální scéně.
/// </summary>
public class AirGame : Game
{
    public const int ScreenWidth = 1280;
    public const int ScreenHeight = 720;

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public SceneManager Scenes { get; } = new();

    /// <summary>Bílý 1×1 pixel – hodí se na kreslení obdélníků, než bude grafika.</summary>
    public Texture2D Pixel { get; private set; }

    public AirGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = ScreenWidth,
            PreferredBackBufferHeight = ScreenHeight,
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = "Simulator cisteni vzduchu";
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Pixel = new Texture2D(GraphicsDevice, 1, 1);
        Pixel.SetData(new[] { Color.White });

        Scenes.ChangeScene(new GameScene(this));
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Scenes.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        Scenes.Draw(gameTime, _spriteBatch);
        base.Draw(gameTime);
    }
}
