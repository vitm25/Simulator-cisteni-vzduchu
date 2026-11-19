using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimulatorCisteniVzduchu.Systems;
using SimulatorCisteniVzduchu.UI;

namespace SimulatorCisteniVzduchu.Scenes;

/// <summary>
/// Hlavní herní obrazovka: kytka na klikání, počítadlo smogu, ukazatel kyslíku
/// a obchod s vylepšeními. Grafika je zatím z obdélníků a koleček – až budou
/// obrázky z Malování, nahradí se v metodách Draw*.
/// </summary>
public class GameScene : Scene
{
    // ---------- Rozložení obrazovky (plátno 1920×1080) ----------
    private const int GroundTop = 820;
    private static readonly Vector2 FlowerBase = new(680, GroundTop);
    private static readonly Rectangle FlowerHitbox = new(530, 300, 300, 530);
    private static readonly Rectangle OxygenBar = new(60, 215, 1240, 50);
    private static readonly Rectangle ShopPanel = new(1360, 30, 530, 1020);
    private const int ShopButtonTop = 120;
    /// <summary>Kam se kreslí pole slunečnic (260×90): mezi skleníky a do pásu za nimi.</summary>
    private static readonly Rectangle[] FieldSlots =
    {
        new(685, 935, 260, 90),
        new(40, 838, 260, 90),
        new(990, 838, 260, 90),
    };

    /// <summary>Ukazatel zvuku vpravo nahoře, nad ukazatelem kyslíku (dá se na něj kliknout).</summary>
    private static readonly Rectangle SoundToggleRect = new(1040, 28, 260, 68);

    private const int ShopButtonHeight = 104;
    private const int ShopButtonGap = 6;

    // ---------- Barvy ----------
    private static readonly Color SmogSky = new(125, 120, 105);
    private static readonly Color CleanSky = new(110, 180, 235);
    private static readonly Color DeadGround = new(115, 105, 75);
    private static readonly Color AliveGround = new(90, 160, 60);
    private static readonly Color Paper = new(245, 238, 215);
    private static readonly Color Ink = new(50, 40, 35);

    private const double AutosaveInterval = 15;

    private GameState _state;
    private Story _story;
    private Villain _villain;
    private readonly DialogBox _dialog = new();
    private readonly VillainToast _toast = new();
    private readonly List<FloatingText> _floatingTexts = new();
    private readonly Random _random = new();
    private readonly List<Vector3> _smogClouds = new(); // X, Y, rychlost
    private float _flowerPulse;
    private double _lastOxygen = -1;
    private double _oxygenTrend;   // vyhlazená změna kyslíku za sekundu
    private float _factoryHaze;    // vyhlazený počet továren (pro opar na obloze)
    private double _autosaveTimer;
    private float _time;
    private bool _finished;
    private readonly Ambience _ambience = new();

    public GameScene(AirGame game) : base(game) { }

    public override void Load()
    {
        VillainArt.Load(Game.Content);
        SceneryArt.Load(Game.Content);

        _state = new GameState(Balance.Load());
        _story = Story.Load();
        _villain = new Villain(_state, _story);

        var save = SaveSystem.Load();
        if (save != null)
        {
            _state.LoadFrom(save);
            _villain.LoadFrom(save.Villain);
        }

        _villain.OnMilestone += milestone =>
            _dialog.Open(_story.VillainName, _story.VillainTitle, milestone.Lines);
        _villain.OnTaunt += text =>
        {
            _toast.Show(_story.VillainName, text);
            Sounds.Play("Sounds/villain_laugh", 0.7f, 0.05f);
        };
        _villain.OnFactoryBuilt += _ =>
        {
            Sounds.Play("Sounds/villain_factory");
            Sounds.Play("Music/music_villain", 0.6f);
        };
        _villain.OnFactoryDestroyed += (factory, bounty) =>
        {
            Sounds.Play("Sounds/factory_sabotage");
            Sounds.Play("Sounds/smog_collect", 0.8f, 0.1f);
            var center = factory.Bounds.Center.ToVector2();
            _floatingTexts.Add(new FloatingText("Továrna zničena!", center - new Vector2(0, 60),
                new Color(255, 230, 120), new Vector2(0, -70)));
            _floatingTexts.Add(new FloatingText("+" + NumberFormat.Whole(bounty) + " smogu", center,
                Color.White, new Vector2(0, -110)));
        };

        for (int i = 0; i < 14; i++)
            _smogClouds.Add(new Vector3(_random.Next(0, 1360), _random.Next(300, 700), _random.Next(15, 45)));
    }

    public override void Unload()
    {
        _ambience.Stop();
        Sounds.StopVoice();
        if (!_finished)
            SaveGame();
    }

    private void SaveGame()
    {
        var save = _state.ToSave();
        save.Villain = _villain.ToSave();
        SaveSystem.Save(save);
    }

    // =====================================================================
    //  UPDATE
    // =====================================================================

    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _time += dt;

        _ambience.Update(dt, _state.Oxygen, _state.Progress, ducked: _dialog.IsOpen);

        // Během příběhového dialogu je hra pozastavená.
        if (_dialog.IsOpen)
        {
            _dialog.Update(dt);
            return;
        }

        _state.Update(dt);
        _villain.Update(dt);
        _toast.Update(dt);

        if (Input.LeftClicked)
            HandleClick(Input.MousePosition);

#if DEBUG
        // Testovací zkratky (jen v Debug buildu):
        //   F2 = přidá 10 % smogu potřebného k výhře,  F3 = záporák hned postaví továrnu.
        if (Input.KeyPressed(Keys.F2))
            _state.AddSmog(_state.Balance.SmogToWin * 0.1);
        if (Input.KeyPressed(Keys.F3))
            _villain.SpawnFactory();
#endif

        _flowerPulse = Math.Max(0, _flowerPulse - dt * 6);
        UpdateOxygenTrend(dt);
        _factoryHaze = MathHelper.Lerp(_factoryHaze, _villain.Factories.Count, 1 - MathF.Exp(-dt * 1.5f));

        for (int i = _floatingTexts.Count - 1; i >= 0; i--)
        {
            _floatingTexts[i].Update(dt);
            if (_floatingTexts[i].IsDead)
                _floatingTexts.RemoveAt(i);
        }

        for (int i = 0; i < _smogClouds.Count; i++)
        {
            var cloud = _smogClouds[i];
            cloud.X += cloud.Z * dt;
            if (cloud.X > 1450) cloud.X = -150;
            _smogClouds[i] = cloud;
        }

        _autosaveTimer += dt;
        if (_autosaveTimer >= AutosaveInterval)
        {
            _autosaveTimer = 0;
            SaveGame();
        }

        // Vítězství až po dočtení případného posledního dialogu záporáka.
        if (_state.IsWon && !_dialog.IsOpen)
        {
            _finished = true;
            SaveSystem.Delete();
            Game.Scenes.ChangeScene(new EndingScene(Game));
        }
    }

    /// <summary>Měří, jak rychle se kyslík skutečně mění (včetně klikání i továren).</summary>
    private void UpdateOxygenTrend(float dt)
    {
        double oxygen = _state.Oxygen;
        if (_lastOxygen >= 0 && dt > 0)
        {
            double change = (oxygen - _lastOxygen) / dt;
            // Velké skoky (F2, odměna za továrnu) do trendu nezapočítáme.
            if (Math.Abs(change) < 5)
                _oxygenTrend = MathHelper.Lerp((float)_oxygenTrend, (float)change, 1 - MathF.Exp(-dt * 1.2f));
        }
        _lastOxygen = oxygen;
    }

    private void HandleClick(Vector2 mouse)
    {
        Point point = mouse.ToPoint();

        if (SoundToggleRect.Contains(point))
        {
            Sounds.ToggleMute();
            Sounds.Play("Sounds/ui_click", 0.6f);
            return;
        }

        // Továrny mají přednost – stojí v krajině vedle kytky.
        var factory = _villain.TryHit(point);
        if (factory != null)
        {
            Sounds.Play("Sounds/ui_click", 0.6f, 0.15f);
            if (factory.Health > 0)
                _floatingTexts.Add(new FloatingText("Bum!", mouse, new Color(255, 180, 120),
                    new Vector2(_random.Next(-40, 41), -130)));
            return;
        }

        if (FlowerHitbox.Contains(point))
        {
            double gained = _state.Click();
            _flowerPulse = 1;
            Sounds.Play("Sounds/click_flower", 0.8f, 0.12f);
            var velocity = new Vector2(_random.Next(-60, 61), -_random.Next(120, 200));
            _floatingTexts.Add(new FloatingText("+" + NumberFormat.Whole(gained), mouse, Color.White, velocity));
            return;
        }

        var upgrades = _state.Balance.Upgrades;
        for (int i = 0; i < upgrades.Count; i++)
        {
            if (!ShopButtonRect(i).Contains(point) || !_state.IsRevealed(upgrades[i], i))
                continue;

            if (_state.TryBuy(upgrades[i]))
            {
                _floatingTexts.Add(new FloatingText("Koupeno!", mouse, new Color(255, 240, 120), new Vector2(0, -90)));
                Sounds.Play("Sounds/buy_upgrade", 0.9f, 0.05f);
                Sounds.Play(upgrades[i].Sound, 0.8f, 0.05f);
            }
            else
            {
                _floatingTexts.Add(new FloatingText("Málo smogu", mouse, new Color(255, 140, 120), new Vector2(0, -90)));
                Sounds.Play("Sounds/not_enough", 0.8f);
            }
            return;
        }
    }

    private static Rectangle ShopButtonRect(int index) => new(
        ShopPanel.X + 15,
        ShopButtonTop + index * (ShopButtonHeight + ShopButtonGap),
        ShopPanel.Width - 30,
        ShopButtonHeight);

    // =====================================================================
    //  DRAW
    // =====================================================================

    public override void Draw(GameTime gameTime, SpriteBatch batch)
    {
        float progress = (float)_state.Progress;
        Game.GraphicsDevice.Clear(Color.Lerp(SmogSky, CleanSky, progress));

        batch.Begin(samplerState: SamplerState.LinearClamp);

        DrawSkyAndGround(batch, progress);
        DrawFactories(batch);           // továrny vzadu, za stromy a ostatními vylepšeními
        DrawUpgradeDecorations(batch);
        DrawFactoryHealthBars(batch);   // ukazatele života ale vždycky navrchu, ať jsou vidět
        DrawFlower(batch);
        DrawHud(batch);
        DrawShop(batch);
        _toast.Draw(batch);

        foreach (var text in _floatingTexts)
            text.Draw(batch);

        _dialog.Draw(batch);

        batch.End();
    }

    /// <summary>Kde se továrna právě kreslí (po zásahu se zatřese).</summary>
    private Rectangle ShakenBounds(Factory factory)
    {
        var bounds = factory.Bounds;
        bounds.Offset((int)(MathF.Sin(_time * 60) * 6 * factory.HitShake), 0);
        return bounds;
    }

    private void DrawFactories(SpriteBatch batch)
    {
        foreach (var factory in _villain.Factories)
            VillainArt.DrawFactory(batch, ShakenBounds(factory), _time, factory.BuildProgress, factory.Slot);
    }

    private void DrawFactoryHealthBars(SpriteBatch batch)
    {
        var mouse = Input.MousePosition.ToPoint();
        foreach (var factory in _villain.Factories)
        {
            if (factory.BuildProgress < 1)
                continue;

            var bounds = ShakenBounds(factory);

            // Ukazatel života nad továrnou
            var bar = new Rectangle(bounds.Center.X - 70, bounds.Y - 30, 140, 16);
            batch.DrawRect(bar, Color.Black * 0.6f);
            var fill = bar;
            fill.Width = (int)(bar.Width * factory.Health / (float)factory.MaxHealth);
            batch.DrawRect(fill, new Color(230, 60, 50));
            batch.DrawFrame(bar, 2, Color.White * 0.8f);

            if (factory.Bounds.Contains(mouse))
                batch.DrawTextCentered(Assets.FontSmall, "Klikej = sabotáž!",
                    new Vector2(bounds.Center.X, bounds.Y - 55), Color.White);
        }
    }

    private void DrawSkyAndGround(SpriteBatch batch, float progress)
    {
        var screen = new Rectangle(0, 0, AirGame.ScreenWidth, AirGame.ScreenHeight);
        bool paintedBackground = SceneryArt.SkySmog != null || SceneryArt.SkyClean != null;

        if (paintedBackground)
        {
            // Zasmogovaná krajina se s čistějším vzduchem postupně prolíná do čisté.
            if (SceneryArt.SkySmog != null)
                batch.Draw(SceneryArt.SkySmog, screen, Color.White);
            if (SceneryArt.SkyClean != null)
                batch.Draw(SceneryArt.SkyClean, screen, Color.White * (SceneryArt.SkySmog != null ? progress : 1f));
        }
        else
        {
            // Slunce se s čistějším vzduchem víc ukazuje.
            batch.DrawCircle(new Vector2(1180, 380), 90, new Color(255, 220, 90) * (0.25f + 0.75f * progress));
        }

        // Továrny zahalí oblohu do hnědého oparu.
        batch.DrawRect(new Rectangle(0, 0, AirGame.ScreenWidth, GroundTop), new Color(70, 50, 30) * (0.09f * _factoryHaze));

        // Mraky smogu postupně mizí – ale továrny je zase přidávají.
        float smogAlpha = Math.Min(0.8f, 0.65f * (1 - progress) + 0.1f * _factoryHaze);
        var smogColor = new Color(70, 65, 55) * smogAlpha;
        foreach (var cloud in _smogClouds)
        {
            var c = new Vector2(cloud.X, cloud.Y);
            if (SceneryArt.SmogCloud != null)
            {
                batch.Draw(SceneryArt.SmogCloud, new Rectangle((int)c.X - 95, (int)c.Y - 60, 190, 115),
                    Color.White * (smogAlpha / 0.8f));
                continue;
            }
            batch.DrawCircle(c, 60, smogColor);
            batch.DrawCircle(c + new Vector2(55, 10), 45, smogColor);
            batch.DrawCircle(c + new Vector2(-50, 15), 40, smogColor);
        }

        if (!paintedBackground)
            batch.DrawRect(new Rectangle(0, GroundTop, AirGame.ScreenWidth, AirGame.ScreenHeight - GroundTop),
                Color.Lerp(DeadGround, AliveGround, progress));
    }

    private void DrawFlower(SpriteBatch batch)
    {
        // Při kliknutí se kytka na chvilku „zmáčkne“.
        float scale = 1 - 0.1f * _flowerPulse;
        bool hover = FlowerHitbox.Contains(Input.MousePosition.ToPoint());
        if (hover) scale += 0.03f;

        if (SceneryArt.Flower != null)
        {
            // Obrázek kytky: pata stonku dole uprostřed, kytka se lehce kolébá ve větru.
            var texture = SceneryArt.Flower;
            var origin = new Vector2(texture.Width / 2f, texture.Height);
            float sway = MathF.Sin(_time * 1.3f) * 0.025f;
            batch.Draw(texture, FlowerBase + new Vector2(0, 10), null, Color.White, sway, origin,
                new Vector2(scale, scale), SpriteEffects.None, 0);
            return;
        }

        Vector2 Pos(float x, float y) => FlowerBase + new Vector2(x, y) * scale;

        var stemColor = new Color(50, 130, 50);
        batch.DrawRect(new Rectangle((int)Pos(-10, 0).X, (int)Pos(0, -320).Y, (int)(20 * scale), (int)(320 * scale)), stemColor);
        batch.DrawCircle(Pos(-45, -150), 38 * scale, new Color(70, 160, 60));
        batch.DrawCircle(Pos(45, -200), 38 * scale, new Color(70, 160, 60));

        Vector2 head = Pos(0, -360);
        var petalColor = new Color(235, 95, 125);
        for (int i = 0; i < 6; i++)
        {
            float angle = MathHelper.TwoPi * i / 6 + _time * 0.3f;
            var offset = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * 75 * scale;
            batch.DrawCircle(head + offset, 58 * scale, petalColor);
        }
        batch.DrawCircle(head, 58 * scale, new Color(255, 210, 60));
    }

    /// <summary>Koupená vylepšení jsou vidět v krajině (zatím jako jednoduché tvary).</summary>
    private void DrawUpgradeDecorations(SpriteBatch batch)
    {
        int CountOf(string id)
        {
            foreach (var upgrade in _state.Balance.Upgrades)
                if (upgrade.Id == id) return _state.Owned(upgrade);
            return 0;
        }

        // Pole slunečnic leží na zemi, proto se kreslí první (stromy a skleníky jsou před nimi).
        int fields = Math.Min(CountOf("pole"), FieldSlots.Length);
        for (int i = 0; i < fields; i++)
        {
            var rect = FieldSlots[i];
            if (SceneryArt.Field != null)
            {
                batch.Draw(SceneryArt.Field, rect, Color.White);
                continue;
            }
            batch.DrawRect(rect, new Color(120, 80, 50));
            for (int k = 0; k < 8; k++)
                batch.DrawCircle(new Vector2(rect.X + 20 + k * 31, rect.Y + 25 + (k % 2) * 30), 12, new Color(255, 200, 30));
        }

        // Stromy (zadní řada, vedle kytky)
        int[] treeSlots = { 120, 1240, 230, 1130, 340, 1020, 450, 910, 175, 1185, 395, 965 };
        int trees = Math.Min(CountOf("strom"), treeSlots.Length);
        for (int i = 0; i < trees; i++)
        {
            float baseY = GroundTop + 20 + (i >= 8 ? 45 : 0);
            var b = new Vector2(treeSlots[i], baseY);
            if (SceneryArt.Tree != null)
            {
                batch.Draw(SceneryArt.Tree, new Rectangle((int)b.X - 55, (int)b.Y - 160, 110, 160), Color.White);
                continue;
            }
            batch.DrawRect(new Rectangle((int)b.X - 9, (int)b.Y - 70, 18, 70), new Color(110, 75, 45));
            batch.DrawCircle(b + new Vector2(0, -105), 55, new Color(45, 120, 45));
        }

        // Skleníky (přední řada)
        int greenhouses = Math.Min(CountOf("sklenik"), 3);
        int[] greenhouseX = { 140, 960, 420 };
        for (int i = 0; i < greenhouses; i++)
        {
            var rect = new Rectangle(greenhouseX[i], 930, 240, 110);
            if (SceneryArt.Greenhouse != null)
            {
                batch.Draw(SceneryArt.Greenhouse, rect, Color.White);
                continue;
            }
            batch.DrawRect(rect, new Color(200, 240, 255) * 0.7f);
            batch.DrawFrame(rect, 4, new Color(240, 250, 255));
        }

        // Roboti jezdí sem a tam po zemi
        int robots = Math.Min(CountOf("robot"), 5);
        for (int i = 0; i < robots; i++)
        {
            float x = 680 + MathF.Sin(_time * 0.5f + i * 1.3f) * 560;
            var body = new Rectangle((int)x - 25, 990 - i * 6, 50, 50);
            if (SceneryArt.Robot != null)
            {
                // Obrázek kouká doprava; když robot jede doleva, otočíme ho.
                bool goingLeft = MathF.Cos(_time * 0.5f + i * 1.3f) < 0;
                batch.Draw(SceneryArt.Robot, body, null, Color.White, 0, Vector2.Zero,
                    goingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                continue;
            }
            batch.DrawRect(body, new Color(150, 155, 165));
            batch.DrawCircle(new Vector2(x, body.Y + 18), 9, new Color(120, 230, 255));
        }

        // Včely létají kolem kytky
        int bees = Math.Min(CountOf("vcely") * 2, 12);
        for (int i = 0; i < bees; i++)
        {
            float angle = _time * (1.5f + i * 0.1f) + i * 0.9f;
            var center = FlowerBase + new Vector2(0, -360);
            var pos = center + new Vector2(MathF.Cos(angle) * (170 + i * 12), MathF.Sin(angle * 1.3f) * (110 + i * 8));
            if (SceneryArt.Bee != null)
            {
                // Obrázek včely kouká doleva; když letí doprava, otočíme ji.
                bool goingRight = MathF.Sin(angle) < 0;
                batch.Draw(SceneryArt.Bee, new Rectangle((int)pos.X - 12, (int)pos.Y - 12, 25, 25), null, Color.White, 0,
                    Vector2.Zero, goingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                continue;
            }
            batch.DrawCircle(pos, 11, new Color(255, 215, 40));
            batch.DrawRect(new Rectangle((int)pos.X - 2, (int)pos.Y - 10, 4, 20), Color.Black * 0.7f);
        }

        // Drony na obloze
        int drones = Math.Min(CountOf("dron"), 5);
        for (int i = 0; i < drones; i++)
        {
            float x = 680 + MathF.Sin(_time * 0.3f + i * 1.7f) * 600;
            float y = 330 + i * 45 + MathF.Sin(_time * 2 + i) * 10;
            if (SceneryArt.Drone != null)
            {
                batch.Draw(SceneryArt.Drone, new Rectangle((int)x - 40, (int)y - 10, 80, 25), Color.White);
                continue;
            }
            batch.DrawRect(new Rectangle((int)x - 30, (int)y, 60, 14), new Color(60, 60, 70));
            batch.DrawRect(new Rectangle((int)x - 40, (int)y - 8, 24, 4), new Color(200, 200, 210));
            batch.DrawRect(new Rectangle((int)x + 16, (int)y - 8, 24, 4), new Color(200, 200, 210));
        }
    }

    private void DrawHud(SpriteBatch batch)
    {
        // Smog (měna)
        batch.DrawCircle(new Vector2(100, 75), 38, new Color(80, 75, 65));
        batch.DrawCircle(new Vector2(122, 62), 26, new Color(95, 90, 80));
        batch.DrawText(Assets.FontBig, NumberFormat.Whole(_state.Smog) + " smogu", new Vector2(160, 30), Color.White);
        batch.DrawText(Assets.FontMain,
            $"+{NumberFormat.Whole(_state.PerClick)} za klik     +{NumberFormat.Rate(_state.PerSecond)} za sekundu",
            new Vector2(62, 120), Color.White);

        // Znečištění od továren záporáka (zarovnané doprava nad ukazatel kyslíku)
        double pollution = _villain.PollutionPerSecond;
        if (pollution > 0)
        {
            string text = $"Továrny ({_villain.Factories.Count}x): -{NumberFormat.Rate(pollution)} za sekundu";
            float width = Assets.FontMain.MeasureString(text).X;
            float pulse = 0.75f + 0.25f * MathF.Sin(_time * 4);
            batch.DrawText(Assets.FontMain, text, new Vector2(OxygenBar.Right - width, 120),
                new Color(255, 110, 90) * pulse);
        }

        // Kyslík
        batch.DrawText(Assets.FontSmall, "Kyslík v atmosféře (cíl 100 %)", new Vector2(62, 180), Color.White);

        // Trend: jestli kyslík zrovna roste, nebo ho továrny stahují dolů.
        bool falling = _oxygenTrend < -0.0005;
        string trend = (_oxygenTrend >= 0 ? "+" : "") + NumberFormat.Percent(_oxygenTrend, 3) + " za sekundu";
        if (falling) trend = "KLESÁ! " + trend;
        float trendWidth = Assets.FontSmall.MeasureString(trend).X;
        batch.DrawText(Assets.FontSmall, trend, new Vector2(OxygenBar.Right - trendWidth, 180),
            falling ? new Color(255, 110, 90) : new Color(170, 255, 170));

        batch.DrawRect(OxygenBar, new Color(40, 40, 40) * 0.6f);
        var fill = OxygenBar;
        fill.Width = (int)(OxygenBar.Width * _state.Oxygen / 100);
        batch.DrawRect(fill, new Color(120, 220, 255));
        float alarm = falling ? 0.5f + 0.5f * MathF.Sin(_time * 8) : 0f;
        batch.DrawFrame(OxygenBar, 4, Color.Lerp(Color.White, new Color(255, 60, 40), alarm));
        batch.DrawTextCentered(Assets.FontMain, "O2: " + NumberFormat.Percent(_state.Oxygen, 2),
            OxygenBar.Center.ToVector2(), Color.White);

        // Nápověda
        if (_state.TotalCleaned < 25)
            batch.DrawTextCentered(Assets.FontBig, "Klikej na kytku!", new Vector2(680, 760), Color.White);

        batch.DrawText(Assets.FontSmall, "F11 – okno / celá obrazovka     Esc – uložit a ukončit",
            new Vector2(20, AirGame.ScreenHeight - 40), Color.White * 0.8f);

        DrawSoundToggle(batch);
    }

    /// <summary>Ikonka reproduktoru vpravo nahoře – ukazuje, jestli je zvuk zapnutý, a dá se na ni kliknout.</summary>
    private void DrawSoundToggle(SpriteBatch batch)
    {
        bool muted = Sounds.Muted;
        bool hover = SoundToggleRect.Contains(Input.MousePosition.ToPoint());

        batch.DrawRect(SoundToggleRect, Color.Black * (hover ? 0.45f : 0.25f));
        batch.DrawFrame(SoundToggleRect, 3, muted ? new Color(230, 80, 70) : new Color(120, 200, 100));

        var iconRect = new Rectangle(SoundToggleRect.X + 8, SoundToggleRect.Y + 10, 48, 48);
        var icon = muted ? SceneryArt.SoundOff : SceneryArt.SoundOn;
        if (icon != null)
            batch.Draw(icon, iconRect, Color.White);

        string label = muted ? "Zvuk vypnutý" : "Zvuk zapnutý";
        batch.DrawText(Assets.FontSmall, label, new Vector2(iconRect.Right + 8, SoundToggleRect.Y + 5),
            muted ? new Color(255, 150, 140) : Color.White);
        batch.DrawText(Assets.FontSmall, "klikni nebo M", new Vector2(iconRect.Right + 8, SoundToggleRect.Y + 35),
            Color.White * 0.65f);
    }

    private void DrawShop(SpriteBatch batch)
    {
        batch.DrawRect(ShopPanel, new Color(60, 50, 40) * 0.85f);
        batch.DrawFrame(ShopPanel, 5, new Color(30, 25, 20));
        batch.DrawTextCentered(Assets.FontBig, "Obchod", new Vector2(ShopPanel.Center.X, ShopPanel.Y + 48), Paper);

        var mouse = Input.MousePosition.ToPoint();
        var upgrades = _state.Balance.Upgrades;
        for (int i = 0; i < upgrades.Count; i++)
        {
            var upgrade = upgrades[i];
            var rect = ShopButtonRect(i);

            if (!_state.IsRevealed(upgrade, i))
            {
                batch.DrawRect(rect, new Color(90, 85, 80));
                batch.DrawTextCentered(Assets.FontBig, "???", rect.Center.ToVector2(), new Color(140, 135, 130));
                continue;
            }

            bool affordable = _state.CanAfford(upgrade);
            bool hover = rect.Contains(mouse);
            Color background = affordable ? Paper : new Color(175, 168, 150);
            if (hover && affordable) background = Color.White;

            batch.DrawRect(rect, background);
            batch.DrawFrame(rect, hover ? 5 : 3, affordable ? new Color(90, 170, 70) : new Color(110, 100, 90));

            Color ink = affordable ? Ink : new Color(90, 80, 70);
            batch.DrawString(Assets.FontMain, upgrade.Name, new Vector2(rect.X + 16, rect.Y + 3), ink);
            batch.DrawString(Assets.FontSmall, upgrade.Description, new Vector2(rect.X + 16, rect.Y + 40), ink * 0.8f);

            string effect = upgrade.PerClick > 0
                ? $"+{NumberFormat.Rate(upgrade.PerClick)}/klik"
                : $"+{NumberFormat.Rate(upgrade.PerSecond)}/s";
            batch.DrawString(Assets.FontSmall, $"Cena: {NumberFormat.Whole(_state.CostOf(upgrade))}   ({effect})",
                new Vector2(rect.X + 16, rect.Y + 70), affordable ? new Color(40, 120, 40) : new Color(150, 60, 50));

            string owned = _state.Owned(upgrade).ToString();
            Vector2 ownedSize = Assets.FontBig.MeasureString(owned);
            batch.DrawString(Assets.FontBig, owned,
                new Vector2(rect.Right - 20 - ownedSize.X, rect.Center.Y - ownedSize.Y / 2), ink * 0.35f);
        }
    }
}
