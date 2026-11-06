using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimulatorCisteniVzduchu.Scenes;

namespace SimulatorCisteniVzduchu;

/// <summary>
/// Vstupní třída hry. Nastaví okno a předává Update/Draw aktuální scéně.
///
/// Hra se vždy kreslí do "virtuálního" plátna 1920×1080 (16:9) a to se pak
/// roztáhne na celou obrazovku. Když monitor není 16:9, doplní se černé pruhy,
/// takže se obraz nikdy nedeformuje. Scény proto vždy počítají se souřadnicemi
/// 0..ScreenWidth a 0..ScreenHeight bez ohledu na skutečné rozlišení.
/// </summary>
public class AirGame : Game
{
    /// <summary>Šířka virtuálního plátna, do kterého kreslí všechny scény.</summary>
    public const int ScreenWidth = 1920;
    /// <summary>Výška virtuálního plátna, do kterého kreslí všechny scény.</summary>
    public const int ScreenHeight = 1080;

    // Velikost okna, když hráč přepne z fullscreenu (F11).
    private const int WindowedWidth = 1280;
    private const int WindowedHeight = 720;

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private RenderTarget2D _canvas;
    private Rectangle _canvasDestination;
    private KeyboardState _previousKeyboard;

    public SceneManager Scenes { get; } = new();

    /// <summary>Bílý 1×1 pixel – hodí se na kreslení obdélníků, než bude grafika.</summary>
    public Texture2D Pixel { get; private set; }

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
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _canvas = new RenderTarget2D(GraphicsDevice, ScreenWidth, ScreenHeight);
        UpdateCanvasDestination();

        Pixel = new Texture2D(GraphicsDevice, 1, 1);
        Pixel.SetData(new[] { Color.White });

        Scenes.ChangeScene(new GameScene(this));
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Escape))
            Exit();

        bool altEnter = keyboard.IsKeyDown(Keys.Enter) && _previousKeyboard.IsKeyUp(Keys.Enter)
                        && (keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt));
        bool f11 = keyboard.IsKeyDown(Keys.F11) && _previousKeyboard.IsKeyUp(Keys.F11);
        if (f11 || altEnter)
            ToggleFullScreen();

        _previousKeyboard = keyboard;

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

    /// <summary>Pozice myši v souřadnicích virtuálního plátna.</summary>
    public Vector2 MouseOnCanvas => ScreenToCanvas(Mouse.GetState().Position);

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
