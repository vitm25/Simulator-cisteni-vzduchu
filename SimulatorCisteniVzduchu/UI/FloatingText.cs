using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SimulatorCisteniVzduchu.UI;

/// <summary>Text typu „+5“, který vyletí z místa kliknutí a postupně zmizí.</summary>
public class FloatingText
{
    private const float Lifetime = 1.0f;

    private readonly string _text;
    private readonly Color _color;
    private Vector2 _position;
    private readonly Vector2 _velocity;
    private float _age;

    public bool IsDead => _age >= Lifetime;

    public FloatingText(string text, Vector2 position, Color color, Vector2 velocity)
    {
        _text = text;
        _position = position;
        _color = color;
        _velocity = velocity;
    }

    public void Update(float deltaSeconds)
    {
        _age += deltaSeconds;
        _position += _velocity * deltaSeconds;
    }

    public void Draw(SpriteBatch batch)
    {
        float alpha = 1 - _age / Lifetime;
        batch.DrawTextCentered(Assets.FontMain, _text, _position, _color * alpha);
    }
}
