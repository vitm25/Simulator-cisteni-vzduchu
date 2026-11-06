using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimulatorCisteniVzduchu.Scenes;
using SimulatorCisteniVzduchu.Systems;
using SimulatorCisteniVzduchu.UI;

namespace SimulatorCisteniVzduchu;


public class AirGame : Game
{
    public const int ScreenWidth = 1920;
    public const int ScreenHeight = 1080;

    // Velikost okna, když hráč přepne z fullscreenu (F11).
    private const int WindowedWidth = 1280;
    private const int WindowedHeight = 720;

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private RenderTarget2D _canvas;
    private Rectangle _canvasDestination;

    public SceneManager Scenes { get; } = new();

    public AirGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            // Bezokrajový fullscreen v rozlišení plochy (bez přepínání režimu monitoru).
            IsFullScreen = true,
            HardwareModeSwitch = false,
            PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
            PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height,
            SynchronizeWithVerticalRetrace = true,
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = "Simulator cisteni vzduchu";
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += (_, _) => UpdateCanvasDestination();

        // Při zavření okna dáme scéně šanci uložit hru.
        Exiting += (_, _) => Scenes.Current?.Unload();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _canvas = new RenderTarget2D(GraphicsDevice, ScreenWidth, ScreenHeight);
        UpdateCanvasDestination();

        Assets.Load(GraphicsDevice, Content);

        Scenes.ChangeScene(new GameScene(this));
    }

    protected override void Update(GameTime gameTime)
    {
        Input.Update(this);

        if (Input.KeyPressed(Keys.Escape))
            Exit();

        bool alt = Input.KeyDown(Keys.LeftAlt) || Input.KeyDown(Keys.RightAlt);
        if (Input.KeyPressed(Keys.F11) || (alt && Input.KeyPressed(Keys.Enter)))
            ToggleFullScreen();

        Scenes.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // 1) Scéna kreslí do virtuálního plátna 1920×1080.
        GraphicsDevice.SetRenderTarget(_canvas);
        Scenes.Draw(gameTime, _spriteBatch);

        // 2) Plátno se roztáhne na obrazovku (černé pruhy, pokud nesedí poměr stran).
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(samplerState: SamplerState.LinearClamp);
        _spriteBatch.Draw(_canvas, _canvasDestination, Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    /// <summary>
    /// Převede pozici na skutečné obrazovce (např. myš) na souřadnice virtuálního
    /// plátna 1920×1080. Scény ji používají pro klikání.
    /// </summary>
    public Vector2 ScreenToCanvas(Point screenPosition)
    {
        if (_canvasDestination.Width == 0 || _canvasDestination.Height == 0)
            return Vector2.Zero;

        return new Vector2(
            (screenPosition.X - _canvasDestination.X) * (float)ScreenWidth / _canvasDestination.Width,
            (screenPosition.Y - _canvasDestination.Y) * (float)ScreenHeight / _canvasDestination.Height);
    }

    private void ToggleFullScreen()
    {
        if (_graphics.IsFullScreen)
        {
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = WindowedWidth;
            _graphics.PreferredBackBufferHeight = WindowedHeight;
        }
        else
        {
            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }

        _graphics.ApplyChanges();
        UpdateCanvasDestination();
    }

    /// <summary>
    /// Spočítá největší obdélník 16:9, který se vejde do okna, a vycentruje ho.
    /// </summary>
    private void UpdateCanvasDestination()
    {
        if (GraphicsDevice == null)
            return;

        int backWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
        int backHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

        float scale = Math.Min((float)backWidth / ScreenWidth, (float)backHeight / ScreenHeight);
        int width = (int)(ScreenWidth * scale);
        int height = (int)(ScreenHeight * scale);

        _canvasDestination = new Rectangle(
            (backWidth - width) / 2,
            (backHeight - height) / 2,
            width,
            height);
    }
}
