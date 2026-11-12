using System.Collections.Generic;

namespace SimulatorCisteniVzduchu.Systems;

/// <summary>Jedna replika v dialogu.</summary>
public class StoryLine
{
    public string Text { get; set; } = "";
    /// <summary>Nepovinný zvuk z Content, např. "Sounds/villain_line_01".</summary>
    public string Sound { get; set; }
}

/// <summary>Příběhový dialog, který se spustí při dosažení určité hladiny kyslíku.</summary>
public class StoryMilestone
{
    public double Oxygen { get; set; }
    public bool StartsFactories { get; set; }
    public List<StoryLine> Lines { get; set; } = new();
}

/// <summary>Texty příběhu a hlášky záporáka. Načítají se z Data/story.json.</summary>
public class Story
{
    public string VillainName { get; set; } = "SMOGOTRON 3000";
    public string VillainTitle { get; set; } = "";
    public List<StoryMilestone> Milestones { get; set; } = new();
    public List<string> FactoryBuiltTaunts { get; set; } = new();
    public List<string> FactoryDestroyedTaunts { get; set; } = new();

    public static Story Load() => DataFiles.Load<Story>("story.json");
}
