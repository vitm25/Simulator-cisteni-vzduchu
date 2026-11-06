# Checklist grafiky a zvuků

Seznam všeho, co je potřeba nakreslit a nahrát. Odškrtávej `[x]`, až bude soubor hotový
a uložený ve správné složce. Názvy souborů jsou návrh – klidně je měň, jen dodrž pravidla níže.

## Pravidla pro soubory

- Názvy: **malá písmena, bez diakritiky a mezer, podtržítka** (`click_flower_01.wav`).
- Zvuky: **WAV, 16-bit PCM, 44100 Hz, mono**, hlasitost normalizovaná (Loudness Normalization, stejná hodnota pro všechny, např. −16 LUFS).
- Obrázky: **PNG**. Celoobrazovkové věci **1920×1080**, objekty v té velikosti, v jaké budou ve hře.
- Průhledné pozadí u objektů (Malování → vrstvy / Odstranit pozadí), jinak sytě růžové pozadí **#FF00FF**.
- Originály (`.aup3`, rozpracované obrázky) ukládej mimo `Content`, třeba do složky `Zdroje/`.

---

## Zvukové efekty → `SimulatorCisteniVzduchu/Content/Sounds/`

Ke každému nahraj ideálně 3 varianty (`_01`, `_02`, `_03`), hra je bude střídat.

### Čištění a klikání
- [ ] `click_flower_01..03.wav` – klik na kytku („fft“, „švih“, luknutí)
- [ ] `smog_collect_01..03.wav` – sebrání smogu / měny („cink“, „plop“)
- [ ] `buy_upgrade.wav` – koupě upgradu („ka-čink!“)
- [ ] `not_enough.wav` – nedostatek smogu na nákup („ee-ee“, „hmm-hm“)
- [ ] `ui_click.wav` – klik na tlačítko v menu

### Prostředí
- [ ] `wind_01..03.wav` – vítr (foukání kolem mikrofonu, „ššššš“)
- [ ] `birds.wav` – ptáci, až bude vzduch čistší (pískání)

### Automatizace
- [ ] `bees.wav` – včely / hmyz („bzzz“, zrychlené)
- [ ] `robot_work.wav` – robotí zahradník („píp-vrr“)
- [ ] `drone.wav` – dron („vrrrrr“, vyšší tón)
- [ ] `greenhouse.wav` – skleník (sklo „cink“ + větráček)
- [ ] `tractor_field.wav` – pole („brrm brrm“)

### Záporák (SMOGOTRON 3000)
- [ ] `villain_laugh.wav` – zlý smích (do hrnku/kýblu + pitch dolů + distortion)
- [ ] `villain_factory.wav` – postavení nové továrny („bum, sssss“)
- [ ] `villain_line_01..05.wav` – hlášky („Smog je budoucnost!“, „Tvoje kytky mě nezastaví!“…)
- [ ] `factory_sabotage.wav` – sabotáž továrny („krach!“)

### Konec hry
- [ ] `match_strike.wav` – škrtnutí zápalky („ššk!“)
- [ ] `match_burn.wav` – hoření plamínku („fff“)
- [ ] `planet_explosion.wav` – výbuch planety („BUUM“ + pitch dolů + basy + reverb)
- [ ] `ending_silence_wind.wav` – ticho po výbuchu, jen slabý vítr ve vesmíru

---

## Hudba → `SimulatorCisteniVzduchu/Content/Music/`

Smyčky nahrané k metronomu (např. 100 BPM, 8 taktů = 19,2 s), střih přesně na dobu.

- [ ] `music_smog.wav` – začátek hry, smutná/zasmogovaná melodie
- [ ] `music_clean.wav` – střed hry, veselejší
- [ ] `music_almost.wav` – konec hry (O₂ nad 90 %), napínavá
- [ ] `music_villain.wav` – motiv záporáka (krátký, při jeho útocích)
- [ ] `music_menu.wav` – hlavní menu (může být jednodušší verze `music_smog`)

---

## Grafika → `SimulatorCisteniVzduchu/Content/Sprites/`

### Pozadí (1920×1080)
- [ ] `bg_sky_smog.png` – šedé zasmogované nebe
- [ ] `bg_sky_medium.png` – napůl čisté nebe
- [ ] `bg_sky_clean.png` – modré čisté nebe
- [ ] `bg_ground.png` – zem / krajina (spodní část obrazovky)
- [ ] `bg_menu.png` – obrázek do hlavního menu / titulní obrazovka

### Vylepšení (tiery)
- [ ] `flower.png` – kytka na klikání (+ případně `flower_clicked.png`)
- [ ] `tree.png` – strom
- [ ] `bee.png` – včela / hmyz (malá, 64×64)
- [ ] `robot.png` – robotí zahradník
- [ ] `greenhouse.png` – skleník
- [ ] `drone.png` – dron
- [ ] `field.png` – pole
- [ ] `forest.png` – les
- [ ] `orbital_vacuum.png` – orbitální vysavač smogu (poslední tier)

### Záporák
- [ ] `villain_portrait.png` – SMOGOTRON 3000 (do dialogů, ~400×400)
- [ ] `villain_angry.png` – naštvaná varianta
- [ ] `factory.png` – továrna, kterou staví
- [ ] `smog_cloud.png` – mráček smogu (letí z továrny / je na klikání)

### UI
- [ ] `icon_smog.png` – ikonka měny (smog, ~64×64)
- [ ] `icon_oxygen.png` – ikonka kyslíku
- [ ] `button.png` – tlačítko (pozadí, natahovatelné)
- [ ] `panel.png` – panel obchodu s upgrady
- [ ] `bar_oxygen.png` – ukazatel O₂ (rám + výplň)

### Font → `SimulatorCisteniVzduchu/Content/Fonts/`
- [ ] Vybrat font pro texty (s českou diakritikou), případně ručně psaný styl

---

## Koncová animace → `SimulatorCisteniVzduchu/Content/Ending/`

5 obrázků **1920×1080**, všechny stejně velké a se stejným rozložením (planeta na stejném místě).

- [ ] `ending_01.png` – Země je zelená, ukazatel O₂ ukazuje 100 %
- [ ] `ending_02.png` – poražený SMOGOTRON vytáhne zápalku
- [ ] `ending_03.png` – škrt! plamínek, kyslík kolem začne žhnout
- [ ] `ending_04.png` – obří výbuch planety
- [ ] `ending_05.png` – trosky ve vesmíru + nápis „Vzduch je čistý. Až moc.“ / KONEC
