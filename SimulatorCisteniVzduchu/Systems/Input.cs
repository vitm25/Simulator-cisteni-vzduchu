using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SimulatorCisteniVzduchu.Systems;

/// <summary>
/// Stav myši a klávesnice pro aktuální snímek. Pozice myši je už přepočítaná
/// na virtuální plátno 1920×1080, takže scény s ní můžou rovnou pracovat.
/// </summary>
public static class Input
{
    private static MouseState _mouse, _previousMouse;
    private static KeyboardState _keyboard, _previousKeyboard;
    private static bool _active;

    /// <summary>Pozice myši na plátně 1920×1080.</summary>
    public static Vector2 MousePosition { get; private set; }

    /// <summary>Levé tlačítko bylo právě teď zmáčknuto (jen v tomhle snímku).</summary>
    public static bool LeftClicked =>
        _active && _mouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released;

    /// <summary>Klávesa byla právě teď zmáčknuta (jen v tomhle snímku).</summary>
    public static bool KeyPressed(Keys key) =>
        _active && _keyboard.IsKeyDown(key) && _previousKeyboard.IsKeyUp(key);

    public static bool KeyDown(Keys key) => _active && _keyboard.IsKeyDown(key);

    public static void Update(AirGame game)
    {
        _previousMouse = _mouse;
        _previousKeyboard = _keyboard;
        _mouse = Mouse.GetState();
        _keyboard = Keyboard.GetState();
        // Když okno není aktivní (hráč je v jiném programu), klikání ignorujeme.
        _active = game.IsActive;
        MousePosition = game.ScreenToCanvas(_mouse.Position);
    }
}
