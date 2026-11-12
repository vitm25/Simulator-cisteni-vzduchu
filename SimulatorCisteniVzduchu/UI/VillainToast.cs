using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SimulatorCisteniVzduchu.UI;

/// <summary>
/// Malá bublina s hláškou záporáka pod ukazatelem kyslíku. Hru nepřerušuje
/// a po pár sekundách sama zmizí.
/// </summary>
public class VillainToast
{
    private const float Duration = 5f;
    private const float FadeTime = 0.5f;
    private const int MaxTextWidth = 700;
    private static readonly Vector2 Position = new(60, 300);

    private string _name = "";
    private string _text = "";
    private float _timeLeft;
    private float _time;

    public void Show(string name, string text)
    {
        _name = name;
        _text = DrawExtensions.WrapText(Assets.FontSmall, text, MaxTextWidth);
        _timeLeft = Duration;
    }

    public void Update(float deltaSeconds)
    {
        _time += deltaSeconds;
        _timeLeft = Math.Max(0, _timeLeft - deltaSeconds);
    }

    public void Draw(SpriteBatch batch)
    {
        if (_timeLeft <= 0)
            return;

        float alpha = Math.Min(1, _timeLeft / FadeTime);
        // Při objevení bublina krátce „vyskočí“.
        float pop = Math.Max(0, (_timeLeft - (Duration - 0.15f)) / 0.15f);
        var offset = new Vector2(0, -pop * 12);

        var portrait = new Rectangle((int)(Position.X + offset.X), (int)(Position.Y + offset.Y), 110, 110);
        batch.DrawRect(portrait, new Color(20, 18, 20) * alpha);
        var head = portrait;
        head.Inflate(-10, -10);
        VillainArt.DrawHead(batch, head, _time, alpha);

        Vector2 nameSize = Assets.FontSmall.MeasureString(_name);
        Vector2 textSize = Assets.FontSmall.MeasureString(_text);
        var bubble = new Rectangle(
            portrait.Right + 15, portrait.Y,
            (int)Math.Max(nameSize.X, textSize.X) + 36,
            (int)Math.Max(portrait.Height, nameSize.Y + textSize.Y + 28));

        batch.DrawRect(bubble, new Color(245, 238, 215) * alpha);
        batch.DrawFrame(bubble, 4, new Color(200, 40, 40) * alpha);
        batch.DrawString(Assets.FontSmall, _name, new Vector2(bubble.X + 18, bubble.Y + 10), new Color(190, 30, 30) * alpha);
        batch.DrawString(Assets.FontSmall, _text, new Vector2(bubble.X + 18, bubble.Y + 14 + nameSize.Y),
            new Color(50, 40, 35) * alpha);
    }
}
