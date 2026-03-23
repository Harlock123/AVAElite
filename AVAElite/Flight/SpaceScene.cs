using System;
using System.Collections.Generic;
using AVAElite.Core;
using AVAElite.Models;
using AVAElite.Universe;

namespace AVAElite.Flight;

public class SpaceScene
{
    public List<SpaceEntity> Entities { get; } = new();
    public SpaceEntity? Station { get; set; }
    public Vector3 PlanetPosition { get; set; }
    public double PlanetRadius { get; set; } = 6000;

    // Player state
    public Vector3 PlayerPosition { get; set; }
    public Matrix3x3 PlayerOrientation { get; set; } = Matrix3x3.Identity;
    public double PlayerSpeed { get; set; }
    public double PlayerEnergy { get; set; } = 255;
    public double PlayerFrontShield { get; set; } = 255;
    public double PlayerRearShield { get; set; } = 255;
    public double LaserTemp { get; set; }
    public double CabinTemp { get; set; }
    public bool MissileArmed { get; set; }
    public SpaceEntity? MissileTarget { get; set; }
    public int ViewDirection { get; set; } // 0=front, 1=rear, 2=left, 3=right
    public double LaserCooldown { get; set; }
    public double _dockingCooldown { get; set; }

    // Column vectors: where local axes point in world space
    public Vector3 PlayerForward => new(PlayerOrientation.M02, PlayerOrientation.M12, PlayerOrientation.M22);
    public Vector3 PlayerUp => new(PlayerOrientation.M01, PlayerOrientation.M11, PlayerOrientation.M21);
    public Vector3 PlayerRight => new(PlayerOrientation.M00, PlayerOrientation.M10, PlayerOrientation.M20);

    public void InitializeSystem(StarSystem system)
    {
        Entities.Clear();

        // Planet ahead in +Z, far but reachable
        PlanetPosition = new Vector3(0, -2000, 30000);
        PlanetRadius = 4000 + system.Radius * 0.5;

        // Station orbiting near the planet, also ahead in +Z
        var stationModel = ShipDefinitions.Coriolis;
        Station = new SpaceEntity
        {
            Model = stationModel,
            Position = new Vector3(1000, 0, 8000),
            Orientation = Matrix3x3.Identity,
            Type = EntityType.Station,
            Energy = 999,
            MaxEnergy = 999,
        };
        Entities.Add(Station);

        // Spawn some NPCs based on system government
        var rng = new Random(system.X * 256 + system.Y);
        int numShips = rng.Next(3, 8);

        // More pirates in anarchy systems
        double pirateChance = system.Government switch
        {
            0 => 0.8,  // Anarchy
            1 => 0.6,  // Feudal
            2 => 0.5,  // Multi-gov
            _ => 0.2,
        };

        for (int i = 0; i < numShips; i++)
        {
            var shipType = PickRandomShip(rng);
            bool hostile = rng.NextDouble() < pirateChance;

            var pos = new Vector3(
                rng.Next(-3000, 3000),
                rng.Next(-1500, 1500),
                rng.Next(1000, 10000)
            );

            var entity = new SpaceEntity
            {
                Model = shipType,
                Position = pos,
                Orientation = RandomOrientation(rng),
                Speed = shipType.MaxSpeed * 0.5,
                Energy = shipType.MaxEnergy,
                MaxEnergy = shipType.MaxEnergy,
                Type = EntityType.Ship,
                Behavior = hostile ? AiBehavior.Pirate : AiBehavior.Trader,
                IsHostile = hostile,
                Bounty = hostile ? shipType.Bounty : 0,
            };
            Entities.Add(entity);
        }
    }

    public void SpawnFromHyperspace(StarSystem system)
    {
        PlayerPosition = new Vector3(0, 0, 0);
        PlayerOrientation = Matrix3x3.Identity; // facing +Z toward station and planet
        PlayerSpeed = 0;
        PlayerEnergy = 255;
        PlayerFrontShield = 255;
        PlayerRearShield = 255;
        LaserTemp = 0;
        CabinTemp = 0;
        ViewDirection = 0;
        InitializeSystem(system);
    }

    public void SpawnFromStation()
    {
        // Player launches away from station (toward planet at +Z)
        // Position well clear of docking range (200 units)
        if (Station != null)
            PlayerPosition = Station.Position + new Vector3(0, 0, 1000);
        else
            PlayerPosition = new Vector3(0, 0, 9000);
        PlayerOrientation = Matrix3x3.Identity; // facing +Z toward planet, away from station
        PlayerSpeed = 10;
        ViewDirection = 0;
        _dockingCooldown = 3.0; // prevent immediate re-dock
    }

    private static ShipModel PickRandomShip(Random rng)
    {
        var ships = new[]
        {
            ShipDefinitions.Sidewinder,
            ShipDefinitions.Viper,
            ShipDefinitions.Mamba,
            ShipDefinitions.Krait,
            ShipDefinitions.CobraMkIII,
            ShipDefinitions.Asp,
            ShipDefinitions.Python,
        };
        return ships[rng.Next(ships.Length)];
    }

    private static Matrix3x3 RandomOrientation(Random rng)
    {
        double yaw = rng.NextDouble() * Math.PI * 2;
        double pitch = (rng.NextDouble() - 0.5) * Math.PI * 0.5;
        return Matrix3x3.RotationY(yaw) * Matrix3x3.RotationX(pitch);
    }
}
