using System;
using System.Globalization;

namespace SimulatorCisteniVzduchu.Systems;

/// <summary>Hezké vypisování velkých čísel: 1 234, 12,5 mil., 3,2 mld.</summary>
public static class NumberFormat
{
    private static readonly CultureInfo Czech = CultureInfo.GetCultureInfo("cs-CZ");

    /// <summary>Celé číslo (smog, ceny).</summary>
    public static string Whole(double value)
    {
        if (value < 1_000_000)
            return Math.Floor(value).ToString("N0", Czech);
        return Large(value);
    }

    /// <summary>Rychlost – u malých čísel ukáže i desetiny (0,5/s).</summary>
    public static string Rate(double value)
    {
        if (value < 10 && value % 1 != 0)
            return value.ToString("0.#", Czech);
        return Whole(value);
    }

    public static string Percent(double value) => value.ToString("0.0", Czech) + " %";

    private static string Large(double value)
    {
        if (value >= 1e12) return (value / 1e12).ToString("0.##", Czech) + " bil.";
        if (value >= 1e9) return (value / 1e9).ToString("0.##", Czech) + " mld.";
        return (value / 1e6).ToString("0.##", Czech) + " mil.";
    }
}
