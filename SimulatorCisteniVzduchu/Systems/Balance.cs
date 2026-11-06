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

/// <summary>Všechna čísla pro vyvážení hry. Načítají se z Data/balance.json.</summary>
public class Balance
{
    public double StartOxygen { get; set; } = 21;
    public double SmogToWin { get; set; } = 3_000_000;
    public double CostGrowth { get; set; } = 1.15;
    public List<UpgradeDefinition> Upgrades { get; set; } = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public static Balance Load()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Data", "balance.json");
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Balance>(json, JsonOptions)
               ?? throw new InvalidDataException("Soubor balance.json je prázdný.");
    }
}
