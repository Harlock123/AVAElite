namespace AVAElite.Universe;

public class StarSystem
{
    public int Index { get; set; }
    public string Name { get; set; } = "";
    public int X { get; set; }          // 0-255
    public int Y { get; set; }          // 0-127
    public int Economy { get; set; }    // 0-7
    public int Government { get; set; } // 0-7
    public int TechLevel { get; set; }  // 1-15
    public int Population { get; set; } // in 0.1 billion
    public int Productivity { get; set; }
    public int Radius { get; set; }
    public string Description { get; set; } = "";

    public static readonly string[] EconomyNames =
    {
        "Rich Industrial", "Average Industrial", "Poor Industrial", "Mainly Industrial",
        "Mainly Agricultural", "Rich Agricultural", "Average Agricultural", "Poor Agricultural"
    };

    public static readonly string[] GovernmentNames =
    {
        "Anarchy", "Feudal", "Multi-Government", "Dictatorship",
        "Communist", "Confederacy", "Democracy", "Corporate State"
    };

    public string EconomyName => EconomyNames[Economy];
    public string GovernmentName => GovernmentNames[Government];
    public string TechLevelString => $"{TechLevel}";
    public string PopulationString => $"{Population / 10}.{Population % 10} Billion";
    public string ProductivityString => $"{Productivity} M Cr";

    public double DistanceTo(StarSystem other)
    {
        double dx = X - other.X;
        double dy = Y - other.Y;
        return System.Math.Sqrt(dx * dx + dy * dy) * 0.4; // scale to LY
    }
}
