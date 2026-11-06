using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SimulatorCisteniVzduchu.Scenes;

/// <summary>
/// Drží aktuální scénu a stará se o přepínání mezi scénami.
/// Přepnutí proběhne až na začátku dalšího Update, aby se scéna
/// nevyměnila uprostřed vlastního Update/Draw.
/// </summary>
public class SceneManager
{
    private Scene _current;
    private Scene _next;

    public Scene Current => _current;

    public void ChangeScene(Scene scene)
    {
        _next = scene;
    }

    public void Update(GameTime gameTime)
    {
        if (_next != null)
        {
            _current?.Unload();
            _current = _next;
            _next = null;
            _current.Load();
        }

        _current?.Update(gameTime);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        _current?.Draw(gameTime, spriteBatch);
    }
}
