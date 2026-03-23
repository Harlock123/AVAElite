using AVAElite.Screens;

namespace AVAElite.Core;

public enum GameScreen
{
    Title,
    Flight,
    GalaxyMap,
    ShortRangeChart,
    SystemInfo,
    Market,
    Status,
    Equipment,
    Launch,
    Hyperspace,
}

public class GameState
{
    public IGameScreen? CurrentScreen { get; set; }

    // Player state
    public int Credits { get; set; } = 1000;    // 100.0 Cr (stored as tenths)
    public int Fuel { get; set; } = 70;          // 7.0 LY (stored as tenths)
    public int MaxFuel { get; set; } = 70;
    public int CurrentGalaxy { get; set; } = 0;
    public int CurrentSystem { get; set; } = 7;  // Lave
    public int SelectedSystem { get; set; } = 7;
    public int LegalStatus { get; set; } = 0;    // 0=Clean
    public int Kills { get; set; } = 0;
    public bool Docked { get; set; } = true;

    // Equipment flags
    public bool HasECM { get; set; }
    public bool HasFuelScoop { get; set; }
    public bool HasEnergyBomb { get; set; }
    public bool HasDockingComputer { get; set; }
    public bool HasGalacticHyperdrive { get; set; }
    public bool HasEscapePod { get; set; }
    public bool HasLargeCargoBay { get; set; }
    public int FrontLaser { get; set; } = 1;   // 0=none, 1=pulse, 2=beam, 3=mining, 4=military
    public int RearLaser { get; set; } = 0;
    public int LeftLaser { get; set; } = 0;
    public int RightLaser { get; set; } = 0;
    public int MissileCount { get; set; } = 3;
    public int MaxMissiles { get; set; } = 4;

    // Cargo
    public int CargoCapacity => HasLargeCargoBay ? 35 : 20;
    public int[] Cargo { get; set; } = new int[17]; // tonnes of each commodity

    public string CombatRating => Kills switch
    {
        < 8 => "Harmless",
        < 16 => "Mostly Harmless",
        < 32 => "Poor",
        < 64 => "Average",
        < 128 => "Above Average",
        < 512 => "Competent",
        < 2560 => "Dangerous",
        < 6400 => "Deadly",
        _ => "---- E L I T E ----",
    };

    public string LegalStatusText => LegalStatus switch
    {
        0 => "Clean",
        < 32 => "Offender",
        _ => "Fugitive",
    };
}
