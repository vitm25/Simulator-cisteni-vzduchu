# Hudebna

Program, který složí 4 skladby do hry a ke každé připraví **podklady k nabroukání**.
Hudba je poskládaná ze zvuků, které jdou udělat pusou (broukání, pískání, „dum“, „bum“, „ts“),
takže ji můžeš snadno nahrát vlastním hlasem a vygenerovanou verzi nahradit.

## Spuštění

Z kořene repozitáře:

```
dotnet run --project Nastroje/Hudebna -- SimulatorCisteniVzduchu/Content/Music Nastroje/Hudebna/podklady
```

> **Pozor:** přepíše `music_*.wav` v `Content/Music`. Až tam budeš mít svou nahranou hudbu,
> program už nespouštěj (nebo jako první argument dej jinou složku).

## Skladby

| Soubor | Kdy hraje | Tempo | Délka |
|---|---|---|---|
| `music_smog` | kyslík pod 50 %, smutná (a moll) | 100 BPM | 8 taktů = 19,2 s |
| `music_clean` | 50–90 %, veselá pískaná (C dur) | 120 BPM | 8 taktů = 16 s |
| `music_almost` | nad 90 %, napínavá (d moll) | 150 BPM | 8 taktů = 12,8 s |
| `music_villain` | jednou při stavbě továrny | 90 BPM | asi 2,9 s |

## Podklady (`podklady/<skladba>/`)

Každý podklad začíná **1 taktem metronomu** na rozjezd, pak jde skladba.

- `01_metronom.wav` – jen ťukání (první doba taktu je vyšší)
- `02_melodie.wav` – melodie, kterou broukáš / pískáš
- `03_basa.wav` – basa („dum“)
- `04_rytmus.wav` – beatbox („bum / pš / ts“)
- `05_cela_skladba.wav` – všechno dohromady, ať víš, jak to má znít
- `noty.txt` – noty po taktech a rytmus po políčkách

Podklady nejsou v gitu, kdykoli se dají vygenerovat znovu. Postup nahrávání je v `NAHRAVANI.md` (blok G).

## Úpravy

Noty jsou v `Program.cs` ve třídě `Songs`, zapsané jako `"E4:2 D4:1 C4:1 | ..."`
(nota+oktáva : délka v dobách, `-:1` = pauza). Zvuky nástrojů jsou v `Synth.cs`.
