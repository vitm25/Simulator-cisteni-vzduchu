using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimulatorCisteniVzduchu.Systems;

namespace SimulatorCisteniVzduchu.UI;

/// <summary>
/// Velké dialogové okno se záporákem. Text se vypisuje písmenko po písmenku,
/// klik (nebo mezerník/Enter) ho dopíše, další klik přejde na další repliku.
/// </summary>
public class DialogBox
{
    private const float CharsPerSecond = 45;
    private static readonly Rectangle Panel = new(180, 650, 1560, 380);
    private static readonly Rectangle PortraitArea = new(Panel.X + 35, Panel.Y + 50, 270, 270);
    private const int TextLeft = 180 + 345;
    private const int TextWidth = 1560 - 345 - 40;

    private List<StoryLine> _lines = new();
    private int _index;
    private string _wrapped = "";
    private float _shownChars;
    private float _time;
    private string _name = "";
    private string _title = "";

    public bool IsOpen { get; private set; }

    public void Open(string name, string title, List<StoryLine> lines)
    {
        if (lines == null || lines.Count == 0)
            return;

        _name = name;
        _title = title;
        _lines = lines;
        IsOpen = true;
        ShowLine(0);
    }

    private void ShowLine(int index)
    {
        _index = index;
        _shownChars = 0;
        _wrapped = DrawExtensions.WrapText(Assets.FontMain, _lines[index].Text, TextWidth);
        Sounds.Play(_lines[index].Sound);
    }

    private bool LineComplete => _shownChars >= _wrapped.Length;

    public void Update(float deltaSeconds)
    {
        if (!IsOpen)
            return;

        _time += deltaSeconds;
        _shownChars += deltaSeconds * CharsPerSecond;

        bool advance = Input.LeftClicked || Input.KeyPressed(Keys.Space) || Input.KeyPressed(Keys.Enter);
        if (!advance)
            return;

        if (!LineComplete)
            _shownChars = _wrapped.Length;          // dopsat celý text
        else if (_index + 1 < _lines.Count)
            ShowLine(_index + 1);                   // další replika
        else
            IsOpen = false;                         // konec dialogu
    }

    public void Draw(SpriteBatch batch)
    {
        if (!IsOpen)
            return;

        // Ztmavení zbytku obrazovky
        batch.DrawRect(new Rectangle(0, 0, AirGame.ScreenWidth, AirGame.ScreenHeight), Color.Black * 0.55f);

        batch.DrawRect(Panel, new Color(35, 30, 32) * 0.96f);
        batch.DrawFrame(Panel, 6, new Color(200, 40, 40));

        batch.DrawRect(PortraitArea, new Color(20, 18, 20));
        var head = PortraitArea;
        head.Inflate(-20, -20);
        VillainArt.DrawHead(batch, head, _time);

        batch.DrawText(Assets.FontMain, _name, new Vector2(TextLeft, Panel.Y + 25), new Color(255, 90, 90));
        float nameWidth = Assets.FontMain.MeasureString(_name).X;
        batch.DrawText(Assets.FontSmall, "– " + _title, new Vector2(TextLeft + nameWidth + 16, Panel.Y + 33),
            Color.White * 0.6f);

        int visible = Math.Min(_wrapped.Length, (int)_shownChars);
        batch.DrawText(Assets.FontMain, _wrapped[..visible], new Vector2(TextLeft, Panel.Y + 100), Color.White);

        batch.DrawText(Assets.FontSmall, $"{_index + 1}/{_lines.Count}",
            new Vector2(TextLeft, Panel.Bottom - 50), Color.White * 0.4f);

        if (LineComplete)
        {
            float blink = 0.5f + 0.5f * MathF.Sin(_time * 6);
            string hint = _index + 1 < _lines.Count ? "klikni >>" : "klikni (konec)";
            Vector2 size = Assets.FontSmall.MeasureString(hint);
            batch.DrawText(Assets.FontSmall, hint, new Vector2(Panel.Right - size.X - 30, Panel.Bottom - 50),
                Color.White * blink);
        }
    }
}
