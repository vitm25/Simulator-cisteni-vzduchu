# Kreslírna

Program, který nakreslí všechny obrázky do hry ve stylu MS Malování:
nevyhlazené hrany, lehce roztřesené čáry, barvy z palety Malování, sprej a písmo Comic Sans.

## Spuštění

Z kořene repozitáře:

```
dotnet run --project Nastroje/Kreslirna -- SimulatorCisteniVzduchu/Content
```

Obrázky se uloží do `Content/Sprites` a `Content/Ending`. Pak stačí spustit hru (F5).

> **Pozor:** program přepíše soubory se stejným názvem. Pokud nějaký obrázek nakreslíš sám
> v Malování, buď ho z programu odstraň (smaž řádek v `Program.cs`), nebo ho po spuštění vrať zpátky.

## Co kde je

- `Art.cs` – „ruka v Malování“: roztřesené tvary, sprej, text, paleta barev.
- `Parts.cs` – díly na víc obrázků: kytka (s obličeji), hlava SMOGOTRONa, planeta.
- `Program.cs` – jednotlivé obrázky. Každý má vlastní funkci, čísla jsou souřadnice v pixelech.

| Obrázek | Velikost |
|---|---|
| `villain_portrait`, `factory`, `flower`, `tree`, `greenhouse` | 230², 170×240, 300×530, 110×160, 240×110 |
| `field`, `robot`, `drone`, `bee`, `smog_cloud` | 260×90, 50², 80×25, 25², 190×115 |
| `bg_sky_smog`, `bg_sky_clean` | 1920×1080 (hra je mezi sebou prolíná podle kyslíku) |
| `ending_01` … `ending_05` | 1920×1080 |

Každý obrázek má pevné „seed“ číslo (druhý parametr `new Art(...)`), takže se při každém
spuštění nakreslí stejně. Když chceš jinou variantu třesu a spreje, změň ho.
