# Simulator cisteni vzduchu
Simulátor čištění vzduchu

Klikací tycoon v C# / MonoGame. Klikáním na kytku čistíš vzduch a sbíráš **smog** (měnu),
za něj kupuješ vylepšení (stromy, včely, roboty, skleníky, pole slunečnic, drony) a zvedáš kyslík
z 21 % na 100 %. Záporák **SMOGOTRON 3000** – AI ředitel firmy Čmoud a.s. – ti staví továrny,
které vzduch zase špiní; klikáním je můžeš zničit. Na 100 % kyslíku planeta vybuchne (koncovka o 5 snímcích).

## Spuštění

Otevři `SimulatorCisteniVzduchu.sln` ve Visual Studiu a dej F5 (potřeba .NET 10).

## Ovládání

| Klávesa | Co dělá |
|---|---|
| Myš | klikání na kytku, továrny a obchod |
| M nebo klik na ikonku vpravo nahoře | zvuk zapnout / vypnout |
| F11 / Alt+Enter | okno / celá obrazovka |
| Esc | uložit a ukončit |
| F2 (jen Debug) | +10 % smogu k výhře – na rychlé testování |
| F3 (jen Debug) | záporák hned postaví továrnu |

Hra se ukládá sama (každých 15 s a při ukončení) do `%LocalAppData%\SimulatorCisteniVzduchu\save.json`.
Smazáním tohoto souboru začneš úplně od začátku.

## Co je nahrané a co vygenerované

Zadání chtělo grafiku z Malování a všechny zvuky nahrané hlasem. Kvůli času to není splněné úplně doslova,
tak ať je jasné, co je co:

**Nahrané mým hlasem**
- všech 17 replik záporáka v dialozích (`dialog_*.wav`)
- klik na kytku (3 varianty), nákup, „málo smogu“ (2 varianty), klik tlačítka, sebrání smogu
- vítr (`wind_01`), škrtnutí a hoření sirky, výbuch planety

**Poskládané z mých nahrávek** (vystřižené nebo upravené)
- smích záporáka (`villain_laugh`) – konec dialogu „…Jen do toho. Hahaha.“
- stavba a zničení továrny (`villain_factory`, `factory_sabotage`) – z nahrávky výbuchu
- tichý vítr ve vesmíru na konci (`ending_silence_wind`) – z nahrávky větru

**Vygenerované počítačem**
- hudba – všechny 4 skladby (`music_smog`, `music_clean`, `music_almost`, `music_villain`).
  Je poskládaná ze „zvuků z pusy“ (broukání, pískání, beatbox) a k nabroukání jsou připravené podklady s notami.
- 6 efektů: včely, ptáci, robot, skleník, traktor (pole), dron
- grafika – kreslená programem (mimo tento repozitář) tak, aby vypadala jako z Malování
  (nevyhlazené hrany, roztřesené čáry, paleta Malování, sprej, Comic Sans)

**Dá se vyměnit:** stačí uložit vlastní nahrávku (WAV 16-bit) do `SimulatorCisteniVzduchu/Content/Sounds`
nebo `Content/Music`, případně vlastní obrázek (PNG) do `Content/Sprites` / `Content/Ending`
**pod stejným jménem** – hra pak použije ten nový. Seznam všech zvuků a kdy hrají je v
`Content/Sounds/_SEZNAM_ZVUKU.md` a `Content/Music/_SEZNAM_HUDBY.md`.
