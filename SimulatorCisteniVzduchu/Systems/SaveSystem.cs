using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SimulatorCisteniVzduchu.Systems;

/// <summary>Data, která se ukládají na disk.</summary>
public class SaveData
{
    public double Smog { get; set; }
    public double TotalCleaned { get; set; }
    public Dictionary<string, int> Owned { get; set; } = new();
}

/// <summary>
/// Ukládá a načítá rozehranou hru do
/// %LocalAppData%\SimulatorCisteniVzduchu\save.json
/// </summary>
public static class SaveSystem
{
    private static readonly string SaveFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "SimulatorCisteniVzduchu");

    private static string SavePath => Path.Combine(SaveFolder, "save.json");

    public static void Save(SaveData data)
    {
        try
        {
            Directory.CreateDirectory(SaveFolder);
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            // Zapisujeme nejdřív do dočasného souboru, ať se save nerozbije, kdyby hra spadla uprostřed zápisu.
            string tempPath = SavePath + ".tmp";
            File.WriteAllText(tempPath, json);
            File.Move(tempPath, SavePath, overwrite: true);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Nepodařilo se uložit hru: {e.Message}");
        }
    }

    public static SaveData Load()
    {
        try
        {
            if (!File.Exists(SavePath))
                return null;
            return JsonSerializer.Deserialize<SaveData>(File.ReadAllText(SavePath));
        }
        catch (Exception e)
        {
            Console.WriteLine($"Nepodařilo se načíst uloženou hru: {e.Message}");
            return null;
        }
    }

    public static void Delete()
    {
        try
        {
            if (File.Exists(SavePath))
                File.Delete(SavePath);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Nepodařilo se smazat uloženou hru: {e.Message}");
        }
    }
}
