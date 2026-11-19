# Simulátor čištění vzduchu

Klikací tycoon v C# / MonoGame. Hráč klikáním na kytku čistí vzduch (měna = **smog**),
kupuje vylepšení a zvedá kyslík z 21 % na 100 %. Pak planeta vybuchne (koncovka o 5 snímcích).
Záporák **SMOGOTRON 3000** (AI ředitel firmy Čmoud a.s.) staví továrny, které kyslík snižují.
Původní zadání: grafika z MS Malování, všechny zvuky nahrané hlasem, vše ve 2D.

## Komunikace a pravidla

- S uživatelem mluv **česky**. Texty ve hře, komentáře v kódu i návody jsou česky.
- **Necommituj a nepushuj**, dokud o to uživatel výslovně nepožádá (commituje sám).
- **Nefoť plochu uživatele** a neposílej syntetické klávesy/klikání do jeho oken. Hru ověřuj
  buildem, headless testy a vyrenderováním snímku do souboru (viz Testování).
- Před spuštěním čehokoli, co může přepsat save (`%LocalAppData%\SimulatorCisteniVzduchu\save.json`),
  save zazálohuj a pak obnov.

## Technika

- .NET 10 (`RollForward Major`), MonoGame 3.8.6.1 DesktopGL, Windows. Řešení: `SimulatorCisteniVzduchu.sln`.
- Build: `dotnet build` ve složce `SimulatorCisteniVzduchu/`.
- Virtuální plátno **1920×1080** (všechny souřadnice v kódu jsou v něm), letterbox do celé obrazovky.
  F11 / Alt+Enter = okno 1280×720. Zem začíná na **y = 820**.
- Scény: `Scenes/GameScene.cs` (hra), `Scenes/EndingScene.cs` (koncovka), `SceneManager`.
- Logika bez kreslení je v `Systems/` (`GameState`, `Villain`, `Balance`, `Story`, `SaveSystem`,
  `Sounds`, `Ambience`). Kreslení a UI v `UI/`.
- Debug klávesy (jen Debug build): **F2** = +10 % smogu k výhře, **F3** = záporák hned postaví továrnu.
  **M** nebo klik na ikonku vpravo nahoře = zvuk zap./vyp.

## Data (dají se měnit bez kódu)

- `Data/balance.json` – ceny a výkony vylepšení (pořadí = pořadí v obchodě, max. 8 se vejde),
  nastavení továren (`factories`). JSON s komentáři.
- `Data/story.json` – dialogy záporáka na milnících kyslíku (0, 24, 40, 60, 80, 95 %),
  hlášky při stavbě/zničení továrny. Každá replika má `sound` (`Sounds/dialog_24_1` = 24 %, 1. replika).

## Obrázky a zvuky – bez MGCB Editoru

- **PNG** do `Content/Sprites` nebo `Content/Ending`, **WAV** do `Content/Sounds` nebo `Content/Music`.
  Csproj je kopíruje k .exe, `Assets.TryLoadTexture` a `Sounds` je načtou přímo ze souboru.
  MGCB (`Content.mgcb`) se používá jen pro fonty.
- Chybějící obrázek → kód nakreslí náhradní tvar. Chybějící zvuk → ticho. Nic nepadá.
- Růžová **#FF00FF** v PNG = průhledná.
- Varianty zvuků `_01`, `_02`… se střídají samy (`Sounds.Play("Sounds/click_flower")`).
- WAV musí být **16-bit PCM**.
- Seznam všech zvuků a kdy hrají: `Content/Sounds/_SEZNAM_ZVUKU.md`, `Content/Music/_SEZNAM_HUDBY.md`.

## Grafika ve stylu Malování

- Všechny obrázky generuje `Nastroje/Kreslirna` (System.Drawing, bez vyhlazování, roztřesené čáry,
  paleta Malování, sprej, Comic Sans):
  `dotnet run --project Nastroje/Kreslirna -- SimulatorCisteniVzduchu/Content`
- **Pozor:** přepíše PNG se stejným názvem – vlastní kresby uživatele by se ztratily.
- Každý obrázek má pevný seed → generování je deterministické.
- `Sablony/` – podklady 1920×1080 s rozložením hry a koncovky + prázdná plátna ve správné velikosti.

## Hudba

- `Nastroje/Hudebna` složí 4 skladby (`music_smog/clean/almost/villain`) z „hlasových“ zvuků
  (broukání, pískání, dum, beatbox) a podklady k nabroukání do `Nastroje/Hudebna/podklady/` (v .gitignore).
  `dotnet run --project Nastroje/Hudebna -- SimulatorCisteniVzduchu/Content/Music Nastroje/Hudebna/podklady`
- Vygenerovaná hudba je dočasná – zadání chce hudbu nahranou hlasem. Uživatel ji nabrouká podle
  podkladů (NAHRAVANI.md, blok G) a přepíše. **Po tom už Hudebnu do Content/Music nespouštět.**

## Dokumenty pro uživatele

- `ASSETS.md` – checklist všech obrázků a zvuků.
- `NAHRAVANI.md` – plán nahrávání zvuků v Audacity 4 (bloky A–G, export WAV 16-bit, štítky).
- Složky `zvuky_A…` až `zvuky_G…` v kořeni jsou pracovní kopie nahrávek uživatele.
- Projekty Audacity (`*.aup3`, `*.aup4`) jsou v `.gitignore` (GitHub limit 100 MB).

## Testování

- Headless testy logiky: konzolový projekt s `ProjectReference` na herní csproj, testuje `Systems/`
  (milníky, továrny, odměny, save/load, dosažitelnost výhry).
- Vizuální kontrola: testovací `Game` podtřída `AirGame`, která po ~90 snímcích uloží `_canvas`
  (přes reflexi) do PNG a skončí; před během se zazálohuje a potom obnoví save, zvuk ztlumit.
- Kouřový test: spustit `bin/Debug/net10.0/SimulatorCisteniVzduchu.exe` na 7 s a ověřit, že běží.
