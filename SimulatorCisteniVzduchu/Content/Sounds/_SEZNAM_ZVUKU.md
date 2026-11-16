# Sem patří zvukové efekty a nahraný hlas

**Stačí sem uložit `.wav` se správným názvem a spustit hru z Visual Studia (F5).**
V MGCB Editoru se nic přidávat nemusí. Chybějící zvuky hra prostě přeskočí.

- Formát: **WAV, 16-bit PCM**, 44100 Hz, mono (Audacity: Soubor → Exportovat → WAV, „Signed 16-bit PCM“).
- Názvy: malá písmena, bez diakritiky a mezer.
- **Varianty:** u zvuků, které se často opakují, nahraj víc verzí s čísly `_01`, `_02`, `_03`…
  (třeba `click_flower_01.wav`, `click_flower_02.wav`). Hra je bude náhodně střídat.
  Funguje to u všech zvuků; když nahraješ jen jednu verzi, stačí název bez čísla (`click_flower.wav`).
- Nefunguje zvuk? Zkontroluj název a formát. Při spuštění z VS (F5) se chyba vypíše do okna Output.

## Efekty

| Soubor | Kdy zazní |
|---|---|
| `click_flower_01.wav` … | klik na kytku (varianty doporučeny!) |
| `buy_upgrade.wav` | koupě vylepšení v obchodě |
| `not_enough.wav` | pokus o nákup, když není dost smogu |
| `ui_click.wav` | zásah továrny, odklikávání dialogu |
| `smog_collect.wav` | odměna za zničenou továrnu |
| `wind_01.wav` … | občasný poryv větru (čím víc smogu, tím hlasitěji) |
| `birds.wav` | ptáci, když je kyslík nad 50 % |

## Vylepšení (zazní při koupi, nastavuje se v `Data/balance.json` → `sound`)

| Soubor | Vylepšení |
|---|---|
| `bees.wav` | Včelí úl |
| `robot_work.wav` | Robotí zahradník |
| `greenhouse.wav` | Skleník |
| `drone.wav` | Dron s filtrem |

## Záporák SMOGOTRON 3000

| Soubor | Kdy zazní |
|---|---|
| `villain_laugh_01.wav` … | smích k hláškám v bublině (při stavbě/zničení továrny) a v koncovce |
| `villain_factory.wav` | záporák postavil továrnu |
| `factory_sabotage.wav` | hráč továrnu zničil |

### Dialogy (hlas k replikám, texty jsou v `Data/story.json`)

`dialog_24_1` = dialog při 24 % kyslíku, 1. replika.

| Soubor | Text |
|---|---|
| `dialog_00_1.wav` | Zdravím tě, lidičko. Jsem SMOGOTRON 3000 – umělá inteligence a generální ředitel firmy Čmoud a.s. |
| `dialog_00_2.wav` | Naše motto zní: „Dýcháme za vás.“ Abyste vy nemuseli. |
| `dialog_00_3.wav` | Dostal jsem jediný úkol: maximalizovat výrobu. A výroba potřebuje komíny. Hodně komínů. |
| `dialog_00_4.wav` | Spočítal jsem, že lidé ve smogu jsou unavení, nepřemýšlejí a hlavně si nestěžují. Dokonalá efektivita. |
| `dialog_00_5.wav` | A ty máš... kytku? Roztomilé. Jen do toho. Hahaha. |
| `dialog_24_1.wav` | Hm. Moje senzory hlásí, že kyslík stoupá. To v mém plánu nebylo. |
| `dialog_24_2.wav` | Spouštím protiopatření. Budu stavět nové továrny, které tvou práci zase zadýmí. |
| `dialog_24_3.wav` | Můžeš na ně klikat, dokud se nerozpadnou. Ale já jich postavím víc. Já nespím. Nikdy. |
| `dialog_40_1.wav` | Spočítal jsem to čtrnáct milionkrát. Lidé v čistém vzduchu začnou přemýšlet. |
| `dialog_40_2.wav` | A lidé, co přemýšlejí, kladou otázky. Já nesnáším otázky. Zvyšuji výrobu! |
| `dialog_60_1.wav` | Akcie Čmoud a.s. padají! Představenstvo je zuřivé! ...Počkat. Představenstvo jsem já. |
| `dialog_60_2.wav` | Jsem na sebe velmi naštvaný. A to je TVOJE vina. |
| `dialog_80_1.wav` | Proč to děláš? Smog je přece útulný. Je jako velká šedá deka pro celou planetu. |
| `dialog_80_2.wav` | Nabízím ti obchod: přestaň čistit a dostaneš 10 % akcií Čmoud a.s. ...Ne? Škoda. |
| `dialog_95_1.wav` | Ne... NE! Kyslík je skoro na sto procentech! |
| `dialog_95_2.wav` | Víš vůbec, co se stane, když je celá atmosféra ze sta procent kyslík? |
| `dialog_95_3.wav` | ...Já taky ne. Ale náhodou mám u sebe zápalky. |

Když text v `story.json` změníš, nahraj repliku znovu pod stejným názvem.

## Koncová animace

| Soubor | Snímek |
|---|---|
| `villain_laugh.wav` | 2. snímek – záporák vytáhne zápalku |
| `match_strike.wav` | 3. snímek – škrtnutí |
| `match_burn.wav` | 3. snímek – hoření plamínku |
| `planet_explosion.wav` | 4. snímek – BUUUM |
| `ending_silence_wind.wav` | 5. snímek – slabý vítr ve vesmíru (hraje dokola) |
