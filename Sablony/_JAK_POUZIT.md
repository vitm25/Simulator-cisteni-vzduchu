# Šablony pro Malování

## Rozložení (podklad, na který se nekreslí)

- `sablona_hra_1920x1080.png` – kde ve hře co je: počítadlo smogu, ukazatel O2, obchod, kytka,
  4 místa pro továrny, pás pro stromy, skleníky a zem (od y = 820). Mřížka je po 100 px.
- `sablona_koncovka_1920x1080.png` – planeta (střed 960, 560, poloměr 250), místo pro nápis,
  místo pro záporáka a pruh dole, který musí na 5. snímku zůstat volný.

**Postup v Malování (Windows 11):**
1. Otevři šablonu.
2. Zapni Vrstvy a přidej novou vrstvu **nad** šablonu.
3. Kresli do nové vrstvy.
4. Šablonovou vrstvu skryj (oko) nebo smaž a dej **Uložit jako** pod správným jménem.

Bez vrstev to jde taky: šablonu si otevři v druhém okně vedle a podle mřížky odměřuj.
Pozici kurzoru ukazuje Malování vlevo dole.

## Prázdná plátna (`prazdna_platna/`)

Plátna mají přesnou velikost i název souboru, jaký čeká hra. Růžová (#FF00FF) bude ve hře průhledná.
Otevři, nakresli a **Uložit jako** do správné složky (ať ti prázdné plátno zůstane):

| Soubor | Velikost | Kam uložit | Poznámka |
|---|---|---|---|
| `villain_portrait.png` | 230×230 | `SimulatorCisteniVzduchu/Content/Sprites/` | ve hře i zmenšený na 90×90 – výrazný obličej |
| `factory.png` | 170×240 | `Content/Sprites/` | komín vpravo nahoře až k hornímu okraji, odtud jde kouř |
| `flower.png` | 300×530 | `Content/Sprites/` | stonek dole uprostřed |
| `tree.png` | 110×160 | `Content/Sprites/` | |
| `greenhouse.png` | 240×110 | `Content/Sprites/` | |
| `field.png` | 260×90 | `Content/Sprites/` | pole slunečnic, leží na zemi |
| `robot.png` | 50×50 | `Content/Sprites/` | |
| `drone.png` | 80×25 | `Content/Sprites/` | |
| `bee.png` | 25×25 | `Content/Sprites/` | |
| `ending_01.png` … `ending_05.png` | 1920×1080 | `Content/Ending/` | černé pozadí = vesmír |

Okraje objektu kresli Tužkou nebo kyblíkem, ne měkkým štětcem – jinak zůstane ve hře růžový lem.

**Do MGCB Editoru se obrázky nepřidávají** – stačí PNG uložit do složky a spustit hru (F5).
Všechny obrázky z tabulky už hra používá. Výchozí verze kreslí program `Nastroje/Kreslirna`
(viz jeho README) – vlastní obrázek se stejným názvem ho jednoduše nahradí.

Šablony vygeneroval skript – kdyby se něco v rozložení hry změnilo, dají se vytvořit znovu.
