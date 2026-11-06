using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimulatorCisteniVzduchu.Systems;
using SimulatorCisteniVzduchu.UI;

namespace SimulatorCisteniVzduchu.Scenes;

/// <summary>
/// Koncová animace o 5 snímcích. Pokud existují obrázky
/// Content/Ending/ending_01.png … ending_05.png (přidané v MGCB Editoru),
/// použijí se. Jinak se nakreslí jednoduchý placeholder.
/// </summary>
public class EndingScene : Scene
{
    private const int FrameCount = 5;
    private const float FrameDuration = 2.2f;

    private static readonly Vector2 PlanetCenter = new(960, 560);
    private const float PlanetRadius = 250;

    private readonly Texture2D[] _frames = new Texture2D[FrameCount];
    private readonly Vector2[] _stars = new Vector2[90];
    private float _time;

    public EndingScene(AirGame game) : base(game) { }

    private int CurrentFrame => Math.Min((int)(_time / FrameDuration), FrameCount - 1);
    private bool AnimationDone => _time >= FrameDuration * FrameCount;

    public override void Load()
    {
        for (int i = 0; i < FrameCount; i++)
            _frames[i] = Assets.TryLoadTexture(Game.Content, $"Ending/ending_{i + 1:00}");

        var random = new Random(42);
        for (int i = 0; i < _stars.Length; i++)
            _stars[i] = new Vector2(random.Next(0, AirGame.ScreenWidth), random.Next(0, AirGame.ScreenHeight));
    }

    public override void Update(GameTime gameTime)
    {
        _time += (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Mezerník / klik přeskočí na další snímek.
        if (!AnimationDone && (Input.KeyPressed(Keys.Space) || Input.LeftClicked))
            _time = (CurrentFrame + 1) * FrameDuration;

        if (AnimationDone && Input.KeyPressed(Keys.Enter))
            Game.Scenes.ChangeScene(new GameScene(Game));
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch)
    {
        Game.GraphicsDevice.Clear(Color.Black);
        batch.Begin(samplerState: SamplerState.LinearClamp);

        int frame = CurrentFrame;
        if (_frames[frame] != null)
            batch.Draw(_frames[frame], new Rectangle(0, 0, AirGame.ScreenWidth, AirGame.ScreenHeight), Color.White);
        else
            DrawPlaceholder(batch, frame);

        if (AnimationDone)
        {
            float blink = 0.6f + 0.4f * MathF.Sin(_time * 4);
            batch.DrawTextCentered(Assets.FontMain, "Enter – nová hra     Esc – konec",
                new Vector2(960, 1020), Color.White * blink);
        }

        batch.End();
    }

    // ---------- Placeholder snímky (dokud nejsou obrázky z Malování) ----------

    private void DrawPlaceholder(SpriteBatch batch, int frame)
    {
        switch (frame)
        {
            case 0: // Země je čistá
                DrawSpace(batch, new Color(15, 20, 50));
                DrawPlanet(batch);
                batch.DrawTextCentered(Assets.FontBig, "Kyslík v atmosféře: 100 %!", new Vector2(960, 140), Color.White);
                batch.DrawTextCentered(Assets.FontMain, "Vzduch na Zemi je dokonale čistý...", new Vector2(960, 900), Color.White);
                break;

            case 1: // Záporák vytáhne zápalku
                DrawSpace(batch, new Color(15, 20, 50));
                DrawPlanet(batch);
                DrawVillain(batch);
                DrawMatch(batch, lit: false);
                batch.DrawTextCentered(Assets.FontMain, "SMOGOTRON 3000: „Když ne smog... tak aspoň OHEŇ!“",
                    new Vector2(960, 140), new Color(255, 120, 120));
                break;

            case 2: // Škrt!
                DrawSpace(batch, new Color(40, 20, 30));
                batch.DrawCircle(PlanetCenter, PlanetRadius + 70, new Color(255, 140, 40) * 0.5f);
                DrawPlanet(batch);
                DrawVillain(batch);
                DrawMatch(batch, lit: true);
                batch.DrawTextCentered(Assets.FontBig, "ŠKRT!", new Vector2(960, 140), new Color(255, 200, 80));
                break;

            case 3: // Výbuch
                DrawSpace(batch, new Color(255, 120, 30));
                batch.DrawCircle(PlanetCenter, 700, new Color(255, 200, 60));
                batch.DrawCircle(PlanetCenter, 480, new Color(255, 240, 150));
                batch.DrawCircle(PlanetCenter, 260, Color.White);
                batch.DrawTextCentered(Assets.FontBig, "BUUUUUM!", PlanetCenter, new Color(200, 40, 20), 1.6f);
                break;

            default: // Trosky
                DrawSpace(batch, Color.Black);
                var random = new Random(7);
                for (int i = 0; i < 40; i++)
                {
                    var pos = PlanetCenter + new Vector2(random.Next(-700, 700), random.Next(-380, 380));
                    var color = i % 3 == 0 ? new Color(60, 140, 60) : new Color(110, 100, 90);
                    batch.DrawCircle(pos, random.Next(6, 30), color);
                }
                batch.DrawTextCentered(Assets.FontBig, "Vzduch je čistý. Až moc.", new Vector2(960, 470), Color.White);
                batch.DrawTextCentered(Assets.FontBig, "KONEC", new Vector2(960, 590), new Color(255, 220, 90));
                break;
        }

        batch.DrawText(Assets.FontSmall,
            $"(placeholder – nakresli Content/Ending/ending_{frame + 1:00}.png)",
            new Vector2(20, 20), Color.White * 0.5f);
    }

    private void DrawSpace(SpriteBatch batch, Color background)
    {
        batch.DrawRect(new Rectangle(0, 0, AirGame.ScreenWidth, AirGame.ScreenHeight), background);
        foreach (var star in _stars)
            batch.DrawRect(new Rectangle((int)star.X, (int)star.Y, 3, 3), Color.White * 0.8f);
    }

    private static void DrawPlanet(SpriteBatch batch)
    {
        batch.DrawCircle(PlanetCenter, PlanetRadius + 18, new Color(170, 230, 255) * 0.6f); // atmosféra
        batch.DrawCircle(PlanetCenter, PlanetRadius, new Color(50, 110, 220));
        batch.DrawCircle(PlanetCenter + new Vector2(-80, -60), 100, new Color(70, 170, 70));
        batch.DrawCircle(PlanetCenter + new Vector2(110, 70), 80, new Color(70, 170, 70));
        batch.DrawCircle(PlanetCenter + new Vector2(-30, 150), 50, new Color(70, 170, 70));
    }

    private static void DrawVillain(SpriteBatch batch)
    {
        var head = new Rectangle(1450, 520, 240, 200);
        batch.DrawRect(head, new Color(120, 120, 130));
        batch.DrawFrame(head, 6, new Color(60, 60, 70));
        batch.DrawCircle(new Vector2(1520, 590), 22, new Color(255, 40, 40));
        batch.DrawCircle(new Vector2(1620, 590), 22, new Color(255, 40, 40));
        batch.DrawRect(new Rectangle(1500, 660, 140, 14), new Color(40, 40, 50));
        batch.DrawRect(new Rectangle(1565, 470, 10, 50), new Color(60, 60, 70)); // anténa
        batch.DrawCircle(new Vector2(1570, 465), 12, new Color(255, 40, 40));
    }

    private static void DrawMatch(SpriteBatch batch, bool lit)
    {
        batch.DrawRect(new Rectangle(1300, 610, 150, 14), new Color(220, 190, 130));
        batch.DrawCircle(new Vector2(1300, 617), 14, new Color(180, 40, 30));
        if (lit)
        {
            batch.DrawCircle(new Vector2(1290, 600), 34, new Color(255, 140, 30));
            batch.DrawCircle(new Vector2(1290, 605), 20, new Color(255, 230, 120));
        }
    }
}
