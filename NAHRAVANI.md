# Plán nahrávání zvuků

Všechno, co je potřeba udělat od zapnutí mikrofonu po zvuk ve hře. Odškrtávej `[x]`.
Celkem **47 souborů** (43 zvuků + 4 hudby), rozdělených do 7 bloků. Každý blok je jedno sezení.

> Zvuky se do MGCB Editoru **nepřidávají**. Stačí uložit `.wav` do správné složky a spustit hru (F5).

---

## Fáze 0 – Příprava (jednou)

### Místnost a mikrofon
- [ ] Ticho: zavřít okno, vypnout větrák, ztlumit notifikace na mobilu.
- [ ] Malá místnost s měkkými věcmi (skříň s oblečením, deka přes hlavu a mikrofon – „deková pevnost“ opravdu funguje).
- [ ] Pusa **10–20 cm** od mikrofonu a **mírně do strany**. Do mikrofonu přímo nemluv, jinak „p“ a „b“ dělají rány.
- [ ] Sluchátka připravená (nutná u hudby, aby se nenahrál metronom).

### Audacity 4
- [ ] **Audio setup** (nahoře uprostřed): vybrat svůj mikrofon jako vstup, kanály mono (1).
- [ ] Test hlasitosti: zkus nejhlasitější zvuk, který budeš nahrávat (smích, „BUUM“). Na ukazateli vpravo mají být špičky
      kolem **-12 až -6**, **nikdy u 0**. Když řve až k 0, ztlum vstup (ve Windows nebo na mikrofonu).
      Přebuzený zvuk už nejde opravit.
- [ ] Projekt uložit **mimo složku hry**, např. `Simulator-cisteni-vzduchu/Zdroje/zvuky_A.aup3`
      (každý blok může mít vlastní projekt). Kdyby se něco nepovedlo, můžeš se k originálu vrátit.

### Test celé cesty (než začneš nahrávat „naostro“)
- [ ] Nahraj jedno rychlé „fft“, pojmenuj ho `click_flower_01` a vyexportuj podle **Fáze 2**.
- [ ] Ve Visual Studiu F5, klikni na kytku. Zahraje? Pak víš, že všechno funguje. Nezahraje? Viz **Když zvuk nehraje** na konci.

---

## Fáze 1 – Postup pro každý blok

Tohle zopakuj u každého bloku z **Fáze 3**:

### 1. Nahrání
1. Nové nahrávání (červené tlačítko / **R**), zastavení čtverec / **mezerník**.
2. Na začátku **5 sekund ticha** (z něj se pak odstraní šum).
3. Každý zvuk nahraj **3–5×** za sebou s vteřinovou pauzou. Nejlepší si pak vybereš,
   nebo jich do hry dáš víc jako varianty (`_01`, `_02`, `_03`).
4. Klidně všechno do jedné dlouhé stopy.

### 2. Úpravy (v tomhle pořadí, menu **Effect**)
1. **Odšumění:** označ těch 5 s ticha → Noise Reduction → *Get Noise Profile*.
   Pak Ctrl+A → Noise Reduction → OK. Začni na **6–12 dB**, víc už zní „roboticky“.
2. **Ořez:** smaž ticho na začátku a konci každého zvuku. U krátkých zvuků je důležité, aby začínaly hned,
   jinak bude klik ve hře působit zpožděně.
3. **Efekty pro konkrétní zvuk** (viz tabulka **Tipy na zvuky** níže).
4. **Fade In / Fade Out** na pár milisekund na úplný začátek a konec každého zvuku – odstraní cvakání.
5. **Hlasitost:** Loudness Normalization na **-16 LUFS**, u všech zvuků stejně. Pak žádný zvuk nebude řvát
   a jiný šeptat.

### 3. Štítky
- Označ myší jeden hotový zvuk a přidej štítek (**Ctrl+B**, případně menu Úpravy → Labels).
- Pojmenuj ho **přesně podle názvu souboru bez `.wav`**, např. `click_flower_01`.
- Názvy: malá písmena, bez diakritiky a mezer, s podtržítky.

### 4. Export
**Soubor → Export audio:**

| Nastavení | Hodnota |
|---|---|
| Formát | **WAV** |
| Kanály | **Mono** (hudba může být i stereo) |
| Vzorkovací frekvence | **44100 Hz** |
| Kódování | **Signed 16-bit PCM** (důležité – jinak ho hra nemusí načíst) |
| Rozsah | **Více souborů, rozdělit podle štítků, pojmenovat podle štítků** |
| Složka | `SimulatorCisteniVzduchu\Content\Sounds` (hudba: `Content\Music`) |

Pokud volba pro víc souborů nebude, jde to i po jednom: označit zvuk → Export audio → jen výběr.

### 5. Kontrola ve hře
- [ ] F5 ve Visual Studiu a vyzkoušet zvuky z bloku (jak, je napsáno u každého bloku).
- [ ] Odškrtat hotové soubory tady i v [ASSETS.md](ASSETS.md).

---

## Fáze 2 – Tipy na zvuky

| Zvuk | Jak na něj |
|---|---|
| Klik na kytku | Krátké „fft“, „švih“, „pšt“ nebo luskání. Víc variant = méně otravné při tisícím kliknutí. |
| Nákup | „Ka-čink!“ nebo „tadáá“. |
| Nedostatek smogu | „Ee-ee“, „hmm-hm“. |
| Sebrání smogu | „Cink“, „plop“ (luknutí prstem o tvář). |
| Vítr | Foukej **kolem** mikrofonu, ne přímo do něj, a měň sílu. Nebo dlouhé „ššššš“ / „fffff“. Pomůže Change Pitch dolů a trocha Reverb. |
| Ptáci | Pískání, krátké trylky. |
| Včely | „Bzzz“ přes zavřenou pusu, zrychlit přes Change Speed. |
| Robot | „Píp-vrr“, zvýšit přes Change Pitch. |
| Dron | „Vrrrrr“, vyšší tón. |
| Skleník | Cinknutí o sklenici + „fff“ větráčku. |
| **Záporák** | Mluv do **hrnku nebo kýblu**, pak Change Pitch dolů (např. -4 půltóny) a Distortion nebo Phaser. Zní to krásně zle. |
| Výbuch planety | Hluboké „BUUM“ zblízka, pak Change Pitch hodně dolů, Bass and Treble (víc basů) a Reverb. |
| Zápalka | „Ššk!“ a krátké „fff“. Nebo opravdová zápalka (opatrně). |

---

## Fáze 3 – Nahrávací bloky

Pořadí je podle toho, co ve hře uslyšíš nejčastěji. Blok A má největší efekt.

### Blok A – Klikání a obchod → `Content/Sounds` (9 souborů)
Vyzkoušení: klikat na kytku, koupit vylepšení, zkusit koupit bez smogu, zničit továrnu (F2 + F3).
- [ ] `click_flower_01.wav`, `click_flower_02.wav`, `click_flower_03.wav` – klik na kytku
- [ ] `buy_upgrade.wav` – koupě vylepšení
- [ ] `not_enough.wav` – málo smogu na nákup
- [ ] `ui_click.wav` – zásah továrny, odklikávání dialogu (tiché, krátké)
- [ ] `smog_collect_01.wav`, `smog_collect_02.wav`, `smog_collect_03.wav` – odměna za zničenou továrnu

### Blok B – Prostředí → `Content/Sounds` (4 soubory)
Vyzkoušení: počkat ve hře, vítr zafouká každých 6–14 s. Ptáci až nad 50 % kyslíku (několikrát F2).
- [ ] `wind_01.wav`, `wind_02.wav`, `wind_03.wav` – poryvy větru (2–4 s)
- [ ] `birds.wav` – ptáci

### Blok C – Vylepšení → `Content/Sounds` (4 soubory)
Vyzkoušení: koupit dané vylepšení (zvuk hraje při koupi navíc k `buy_upgrade`).
- [ ] `bees.wav` – Včelí úl
- [ ] `robot_work.wav` – Robotí zahradník
- [ ] `greenhouse.wav` – Skleník
- [ ] `drone.wav` – Dron s filtrem

### Blok D – Záporák: efekty → `Content/Sounds` (5 souborů)
Vyzkoušení: F2 (na 24 %), pak F3 = postaví továrnu; klikáním ji zničit.
- [ ] `villain_laugh_01.wav`, `villain_laugh_02.wav`, `villain_laugh_03.wav` – zlý smích (k hláškám v bublině a v koncovce)
- [ ] `villain_factory.wav` – postavení továrny („bum, sssss“)
- [ ] `factory_sabotage.wav` – zničení továrny („krach!“)

### Blok E – Záporák: dialogy → `Content/Sounds` (17 souborů)
**Všechny repliky nahraj v jednom sezení a se stejnými efekty** (stejný hrnek, stejný pitch, stejná distortion),
jinak bude záporák znít pokaždé jinak. Nastavení efektů si zapiš.
Vyzkoušení: nová hra = úvodní dialog; F2 posouvá na další milníky (24, 40, 60, 80, 95 %).

Texty jsou ze souboru `Data/story.json`. Když tam text změníš, nahraj repliku znovu pod stejným názvem.

| ✓ | Soubor | Text |
|---|---|---|
| [ ] | `dialog_00_1` | Zdravím tě, lidičko. Jsem SMOGOTRON 3000 – umělá inteligence a generální ředitel firmy Čmoud a.s. |
| [ ] | `dialog_00_2` | Naše motto zní: „Dýcháme za vás.“ Abyste vy nemuseli. |
| [ ] | `dialog_00_3` | Dostal jsem jediný úkol: maximalizovat výrobu. A výroba potřebuje komíny. Hodně komínů. |
| [ ] | `dialog_00_4` | Spočítal jsem, že lidé ve smogu jsou unavení, nepřemýšlejí a hlavně si nestěžují. Dokonalá efektivita. |
| [ ] | `dialog_00_5` | A ty máš... kytku? Roztomilé. Jen do toho. Hahaha. |
| [ ] | `dialog_24_1` | Hm. Moje senzory hlásí, že kyslík stoupá. To v mém plánu nebylo. |
| [ ] | `dialog_24_2` | Spouštím protiopatření. Budu stavět nové továrny, které tvou práci zase zadýmí. |
| [ ] | `dialog_24_3` | Můžeš na ně klikat, dokud se nerozpadnou. Ale já jich postavím víc. Já nespím. Nikdy. |
| [ ] | `dialog_40_1` | Spočítal jsem to čtrnáct milionkrát. Lidé v čistém vzduchu začnou přemýšlet. |
| [ ] | `dialog_40_2` | A lidé, co přemýšlejí, kladou otázky. Já nesnáším otázky. Zvyšuji výrobu! |
| [ ] | `dialog_60_1` | Akcie Čmoud a.s. padají! Představenstvo je zuřivé! ...Počkat. Představenstvo jsem já. |
| [ ] | `dialog_60_2` | Jsem na sebe velmi naštvaný. A to je TVOJE vina. |
| [ ] | `dialog_80_1` | Proč to děláš? Smog je přece útulný. Je jako velká šedá deka pro celou planetu. |
| [ ] | `dialog_80_2` | Nabízím ti obchod: přestaň čistit a dostaneš 10 % akcií Čmoud a.s. ...Ne? Škoda. |
| [ ] | `dialog_95_1` | Ne... NE! Kyslík je skoro na sto procentech! |
| [ ] | `dialog_95_2` | Víš vůbec, co se stane, když je celá atmosféra ze sta procent kyslík? |
| [ ] | `dialog_95_3` | ...Já taky ne. Ale náhodou mám u sebe zápalky. |

Tip: text se ve hře vypisuje rychlostí asi 45 znaků za sekundu. Když budeš mluvit podobným tempem,
hlas a text doběhnou zhruba spolu.

### Blok F – Koncová animace → `Content/Sounds` (4 soubory)
Vyzkoušení: mačkat F2 (a odklikávat dialogy), dokud kyslík nedojde na 100 % → koncovka.
- [ ] `match_strike.wav` – škrtnutí zápalky (3. snímek)
- [ ] `match_burn.wav` – hoření plamínku (3. snímek)
- [ ] `planet_explosion.wav` – výbuch planety (4. snímek)
- [ ] `ending_silence_wind.wav` – slabý vítr ve vesmíru, hraje dokola (5. snímek). Střihni tak, aby smyčka navazovala.
- (2. snímek použije `villain_laugh` z bloku D)

### Blok G – Hudba → `Content/Music` (4 soubory)
Nejpracnější blok, nech si ho na konec. Stačí začít s `music_smog.wav` – dokud ostatní chybí, hraje ve hře místo nich.

**Postup:**
1. Menu **Vytvoření** → Rhythm Track: metronom, třeba **100 BPM**. Poslouchej ho **ve sluchátkách**.
2. Každou vrstvu nahraj do **samostatné stopy** (Shift+R nahraje novou stopu souběžně s ostatními):
   - [ ] beatbox („bum-ts-bum-ts“)
   - [ ] basa (hluboké „dum dum“)
   - [ ] melodie (broukání nebo pískání)
3. Délka smyčky: **8 taktů při 100 BPM = přesně 19,2 s**. Střih přesně na dobu, aby smyčka navazovala.
   Pomůže výběr *At Zero Crossings* (klávesa **Z**) – střih pak necvakne.
4. Před exportem **smaž stopu s metronomem**.
5. Vyzkoušej smyčku v Audacity tlačítkem pro opakované přehrávání (šipka dokola v horní liště). Nesmí cvaknout ani poskočit.
6. Export: **jeden soubor** (ne podle štítků), Audacity stopy smíchá dohromady. Může být stereo, 16-bit PCM.

**Skladby:**
- [ ] `music_smog.wav` – kyslík pod 50 %, smutná a zasmogovaná
- [ ] `music_clean.wav` – kyslík 50–90 %, veselejší
- [ ] `music_almost.wav` – kyslík nad 90 %, napínavá
- [ ] `music_villain.wav` – krátký motiv záporáka (pár vteřin), zazní jednou při stavbě továrny. Nehraje dokola.

Vyzkoušení: hudba hraje hned po spuštění; F2 posouvá kyslík a hudba se přelije do další skladby.
Během dialogu se ztiší.

---

## Až budou potřeba (zatím ne)

Tyhle zvuky jsou v checklistu, ale ve hře zatím nejsou místa, kde by zazněly:
- `tractor_field.wav` – pole (vylepšení zatím neexistuje)
- `music_menu.wav` – hlavní menu (zatím neexistuje)

---

## Když zvuk nehraje

1. **Název:** přesně podle seznamu, malá písmena, koncovka `.wav`. Žádné `Click_Flower 01.wav`.
2. **Složka:** efekty `SimulatorCisteniVzduchu/Content/Sounds`, hudba `SimulatorCisteniVzduchu/Content/Music`.
3. **Formát:** WAV, **Signed 16-bit PCM**. Jiné kódování (24-bit, 32-bit float) hra nemusí načíst.
4. **Build:** soubory se zkopírují k `.exe` až při buildu. Spouštěj přes F5; když to nepomůže, Sestavení → Znovu sestavit.
5. **Výpis chyb:** při spuštění přes F5 se vadný soubor vypíše do okna Výstup (Output) jako `[Sounds] Nejde načíst …`.
6. **Ztlumeno:** klávesa **M** vypíná všechny zvuky. Zkus ji zmáčknout.
