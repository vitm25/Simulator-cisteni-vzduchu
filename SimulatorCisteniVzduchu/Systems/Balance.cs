using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SimulatorCisteniVzduchu.Systems;

/// <summary>Jedno vylepšení z obchodu, tak jak je popsané v Data/balance.json.</summary>
public class UpgradeDefinition
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public double BaseCost { get; set; }
    public double PerClick { get; set; }
    public double PerSecond { get; set; }
}

/// <summary>Nastavení továren záporáka (sekce "factories" v balance.json).</summary>
public class FactoryBalance
{
    public double SpawnIntervalStart { get; set; } = 45;
    public double SpawnIntervalEnd { get; set; } = 18;
    public int MaxCount { get; set; } = 4;
    public double PollutionFraction { get; set; } = 0.25;
    /// <summary>O kolik procentních bodů kyslíku za sekundu stáhne jedna továrna minimálně.</summary>
    public double MinOxygenDrain { get; set; } = 0.01;
    public int BaseHealth { get; set; } = 12;
    public int HealthGrowth { get; set; } = 2;
    public int MaxHealth { get; set; } = 40;
    public double BountySeconds { get; set; } = 20;
}

/// <summary>Všechna čísla pro vyvážení hry. Načítají se z Data/balance.json.</summary>
public class Balance
{
    public double StartOxygen { get; set; } = 21;
    public double SmogToWin { get; set; } = 3_000_000;
    public double CostGrowth { get; set; } = 1.15;
    public FactoryBalance Factories { get; set; } = new();
    public List<UpgradeDefinition> Upgrades { get; set; } = new();

    public static Balance Load() => DataFiles.Load<Balance>("balance.json");
}

/// <summary>Načítání JSON souborů ze složky Data (vedle .exe).</summary>
public static class DataFiles
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public static T Load<T>(string fileName)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Data", fileName);
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json, JsonOptions)
               ?? throw new InvalidDataException($"Soubor {fileName} je prázdný.");
    }
}
