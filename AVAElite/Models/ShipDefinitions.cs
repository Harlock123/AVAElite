using System.Collections.Generic;
using AVAElite.Core;

namespace AVAElite.Models;

public static class ShipDefinitions
{
    public static ShipModel CobraMkIII { get; } = CreateCobraMkIII();
    public static ShipModel Sidewinder { get; } = CreateSidewinder();
    public static ShipModel Viper { get; } = CreateViper();
    public static ShipModel Mamba { get; } = CreateMamba();
    public static ShipModel Krait { get; } = CreateKrait();
    public static ShipModel Asp { get; } = CreateAsp();
    public static ShipModel Python { get; } = CreatePython();
    public static ShipModel Coriolis { get; } = CreateCoriolis();
    public static ShipModel Missile { get; } = CreateMissile();
    public static ShipModel Asteroid { get; } = CreateAsteroid();
    public static ShipModel CargoCan { get; } = CreateCargoCan();
    public static ShipModel Thargoid { get; } = CreateThargoid();
    public static ShipModel EscapePod { get; } = CreateEscapePod();

    private static ShipModel CreateCobraMkIII()
    {
        // Based on the original BBC Elite Cobra Mk III blueprint
        // Z = forward (nose), Y = up, X = right
        var vertices = new[]
        {
            new Vector3(32, 0, 76),     // 0: nose right
            new Vector3(-32, 0, 76),    // 1: nose left
            new Vector3(0, 26, 24),     // 2: top center
            new Vector3(-120, 0, -8),   // 3: left wing tip
            new Vector3(120, 0, -8),    // 4: right wing tip
            new Vector3(-62, 0, -44),   // 5: rear left
            new Vector3(62, 0, -44),    // 6: rear right
            new Vector3(0, -26, 24),    // 7: bottom center
        };

        // 12 triangular faces forming a convex hull
        // Top half: vertex 2 connects to the equatorial ring
        // Bottom half: vertex 7 connects to the equatorial ring
        var faces = new[]
        {
            new[] { 2, 0, 4 },   // top front right
            new[] { 2, 4, 6 },   // top right rear
            new[] { 2, 6, 5 },   // top rear
            new[] { 2, 5, 3 },   // top left rear
            new[] { 2, 3, 1 },   // top front left
            new[] { 2, 1, 0 },   // top nose
            new[] { 7, 4, 0 },   // bottom front right
            new[] { 7, 6, 4 },   // bottom right rear
            new[] { 7, 5, 6 },   // bottom rear
            new[] { 7, 3, 5 },   // bottom left rear
            new[] { 7, 1, 3 },   // bottom front left
            new[] { 7, 0, 1 },   // bottom nose
        };

        var model = ShipModel.Create("Cobra Mk III", vertices, faces);
        model.MaxSpeed = 28;
        model.MaxEnergy = 150;
        model.MissileCount = 4;
        model.LaserPower = 2;
        model.CargoCapacity = 20;
        return model;
    }

    private static ShipModel CreateSidewinder()
    {
        var vertices = new[]
        {
            new Vector3(0, 0, 48),      // 0: nose
            new Vector3(-36, 0, -24),   // 1: left wing
            new Vector3(36, 0, -24),    // 2: right wing
            new Vector3(0, -12, 0),     // 3: top
            new Vector3(0, 12, 0),      // 4: bottom
            new Vector3(0, 0, -36),     // 5: tail
        };

        var faces = new[]
        {
            new[] { 0, 3, 2 },
            new[] { 0, 1, 3 },
            new[] { 0, 2, 4 },
            new[] { 0, 4, 1 },
            new[] { 5, 2, 3 },
            new[] { 5, 3, 1 },
            new[] { 5, 4, 2 },
            new[] { 5, 1, 4 },
        };

        var model = ShipModel.Create("Sidewinder", vertices, faces);
        model.MaxSpeed = 37;
        model.MaxEnergy = 70;
        model.MissileCount = 0;
        model.LaserPower = 2;
        model.Bounty = 50;
        return model;
    }

    private static ShipModel CreateViper()
    {
        var vertices = new[]
        {
            new Vector3(0, 0, 60),      // 0: nose
            new Vector3(-48, 0, -24),   // 1: left wing
            new Vector3(48, 0, -24),    // 2: right wing
            new Vector3(0, -16, 8),     // 3: top
            new Vector3(0, 16, 8),      // 4: bottom
            new Vector3(-12, 0, -40),   // 5: rear left
            new Vector3(12, 0, -40),    // 6: rear right
        };

        var faces = new[]
        {
            new[] { 0, 3, 2 },
            new[] { 0, 1, 3 },
            new[] { 0, 2, 4 },
            new[] { 0, 4, 1 },
            new[] { 3, 6, 2 },
            new[] { 3, 1, 5 },
            new[] { 3, 5, 6 },
            new[] { 4, 2, 6 },
            new[] { 4, 5, 1 },
            new[] { 4, 6, 5 },
        };

        var model = ShipModel.Create("Viper", vertices, faces);
        model.MaxSpeed = 32;
        model.MaxEnergy = 120;
        model.MissileCount = 1;
        model.LaserPower = 2;
        model.Bounty = 0;
        return model;
    }

    private static ShipModel CreateMamba()
    {
        var vertices = new[]
        {
            new Vector3(0, 0, 64),      // 0: nose
            new Vector3(-64, 0, -32),   // 1: left wing
            new Vector3(64, 0, -32),    // 2: right wing
            new Vector3(0, -8, -8),     // 3: top
            new Vector3(0, 8, -8),      // 4: bottom
            new Vector3(0, 0, -48),     // 5: tail
        };

        var faces = new[]
        {
            new[] { 0, 3, 2 },
            new[] { 0, 1, 3 },
            new[] { 0, 2, 4 },
            new[] { 0, 4, 1 },
            new[] { 5, 2, 3 },
            new[] { 5, 3, 1 },
            new[] { 5, 4, 2 },
            new[] { 5, 1, 4 },
        };

        var model = ShipModel.Create("Mamba", vertices, faces);
        model.MaxSpeed = 30;
        model.MaxEnergy = 90;
        model.MissileCount = 2;
        model.LaserPower = 3;
        model.Bounty = 150;
        return model;
    }

    private static ShipModel CreateKrait()
    {
        var vertices = new[]
        {
            new Vector3(0, 0, 50),      // 0: nose
            new Vector3(-44, 0, -20),   // 1: left wing
            new Vector3(44, 0, -20),    // 2: right wing
            new Vector3(0, -10, 0),     // 3: top
            new Vector3(0, 10, 0),      // 4: bottom
            new Vector3(0, 0, -40),     // 5: tail
        };

        var faces = new[]
        {
            new[] { 0, 3, 2 },
            new[] { 0, 1, 3 },
            new[] { 0, 2, 4 },
            new[] { 0, 4, 1 },
            new[] { 5, 2, 3 },
            new[] { 5, 3, 1 },
            new[] { 5, 4, 2 },
            new[] { 5, 1, 4 },
        };

        var model = ShipModel.Create("Krait", vertices, faces);
        model.MaxSpeed = 30;
        model.MaxEnergy = 80;
        model.MissileCount = 0;
        model.LaserPower = 2;
        model.Bounty = 100;
        return model;
    }

    private static ShipModel CreateAsp()
    {
        var vertices = new[]
        {
            new Vector3(0, 0, 70),      // 0: nose
            new Vector3(-60, 0, -20),   // 1: left wing
            new Vector3(60, 0, -20),    // 2: right wing
            new Vector3(0, -18, 10),    // 3: top
            new Vector3(0, 18, 10),     // 4: bottom
            new Vector3(-30, -8, -50),  // 5: rear top left
            new Vector3(30, -8, -50),   // 6: rear top right
            new Vector3(-30, 8, -50),   // 7: rear bottom left
            new Vector3(30, 8, -50),    // 8: rear bottom right
        };

        var faces = new[]
        {
            new[] { 0, 3, 2 },
            new[] { 0, 1, 3 },
            new[] { 0, 2, 4 },
            new[] { 0, 4, 1 },
            new[] { 3, 6, 2 },
            new[] { 3, 1, 5 },
            new[] { 4, 2, 8 },
            new[] { 4, 7, 1 },
            new[] { 3, 5, 6 },
            new[] { 4, 8, 7 },
            new[] { 5, 7, 8, 6 },
        };

        var model = ShipModel.Create("Asp Mk II", vertices, faces);
        model.MaxSpeed = 40;
        model.MaxEnergy = 150;
        model.MissileCount = 1;
        model.LaserPower = 4;
        model.Bounty = 200;
        return model;
    }

    private static ShipModel CreatePython()
    {
        // Python: large, bulky trading ship
        var vertices = new[]
        {
            new Vector3(0, 0, 80),      // 0: nose
            new Vector3(-40, -20, 20),  // 1: upper left
            new Vector3(40, -20, 20),   // 2: upper right
            new Vector3(-40, 20, 20),   // 3: lower left
            new Vector3(40, 20, 20),    // 4: lower right
            new Vector3(-40, -20, -60), // 5: rear upper left
            new Vector3(40, -20, -60),  // 6: rear upper right
            new Vector3(-40, 20, -60),  // 7: rear lower left
            new Vector3(40, 20, -60),   // 8: rear lower right
        };

        var faces = new[]
        {
            new[] { 0, 2, 1 },     // top nose
            new[] { 0, 4, 2 },     // right nose
            new[] { 0, 3, 4 },     // bottom nose
            new[] { 0, 1, 3 },     // left nose
            new[] { 1, 2, 6, 5 },  // top
            new[] { 2, 4, 8, 6 },  // right
            new[] { 4, 3, 7, 8 },  // bottom
            new[] { 3, 1, 5, 7 },  // left
            new[] { 5, 6, 8, 7 },  // rear
        };

        var model = ShipModel.Create("Python", vertices, faces);
        model.MaxSpeed = 20;
        model.MaxEnergy = 250;
        model.MissileCount = 3;
        model.LaserPower = 3;
        model.Bounty = 0;
        model.CargoCapacity = 100;
        return model;
    }

    private static ShipModel CreateCoriolis()
    {
        // Coriolis station: a cube, rotating
        double s = 80;
        var vertices = new[]
        {
            new Vector3(-s, -s, -s), // 0
            new Vector3(s, -s, -s),  // 1
            new Vector3(s, s, -s),   // 2
            new Vector3(-s, s, -s),  // 3
            new Vector3(-s, -s, s),  // 4
            new Vector3(s, -s, s),   // 5
            new Vector3(s, s, s),    // 6
            new Vector3(-s, s, s),   // 7
        };

        var faces = new[]
        {
            new[] { 4, 5, 6, 7 },   // front (+Z)
            new[] { 1, 0, 3, 2 },   // back (-Z)
            new[] { 0, 4, 7, 3 },   // left (-X)
            new[] { 5, 1, 2, 6 },   // right (+X)
            new[] { 0, 1, 5, 4 },   // top (-Y)
            new[] { 7, 6, 2, 3 },   // bottom (+Y)
        };

        var model = ShipModel.Create("Coriolis", vertices, faces);
        model.MaxSpeed = 0;
        model.MaxEnergy = 999;
        return model;
    }

    private static ShipModel CreateMissile()
    {
        var vertices = new[]
        {
            new Vector3(0, 0, 30),    // 0: nose
            new Vector3(-4, -4, -14), // 1
            new Vector3(4, -4, -14),  // 2
            new Vector3(4, 4, -14),   // 3
            new Vector3(-4, 4, -14),  // 4
        };

        var faces = new[]
        {
            new[] { 0, 2, 1 },
            new[] { 0, 3, 2 },
            new[] { 0, 4, 3 },
            new[] { 0, 1, 4 },
            new[] { 1, 2, 3, 4 },
        };

        var model = ShipModel.Create("Missile", vertices, faces);
        model.MaxSpeed = 44;
        model.MaxEnergy = 2;
        return model;
    }

    private static ShipModel CreateAsteroid()
    {
        // Irregular octahedron
        var vertices = new[]
        {
            new Vector3(0, -40, 0),   // 0: top
            new Vector3(0, 40, 0),    // 1: bottom
            new Vector3(-50, 5, -10), // 2: left
            new Vector3(50, -5, -10), // 3: right
            new Vector3(10, 10, 50),  // 4: front
            new Vector3(-10, -10, -50), // 5: back
        };

        var faces = new[]
        {
            new[] { 0, 4, 3 },
            new[] { 0, 2, 4 },
            new[] { 0, 3, 5 },
            new[] { 0, 5, 2 },
            new[] { 1, 3, 4 },
            new[] { 1, 4, 2 },
            new[] { 1, 5, 3 },
            new[] { 1, 2, 5 },
        };

        var model = ShipModel.Create("Asteroid", vertices, faces);
        model.MaxSpeed = 10;
        model.MaxEnergy = 60;
        return model;
    }

    private static ShipModel CreateCargoCan()
    {
        double w = 8, h = 8, l = 16;
        var vertices = new[]
        {
            new Vector3(-w, -h, l),  // 0
            new Vector3(w, -h, l),   // 1
            new Vector3(w, h, l),    // 2
            new Vector3(-w, h, l),   // 3
            new Vector3(-w, -h, -l), // 4
            new Vector3(w, -h, -l),  // 5
            new Vector3(w, h, -l),   // 6
            new Vector3(-w, h, -l),  // 7
        };

        var faces = new[]
        {
            new[] { 0, 1, 2, 3 },
            new[] { 5, 4, 7, 6 },
            new[] { 4, 0, 3, 7 },
            new[] { 1, 5, 6, 2 },
            new[] { 4, 5, 1, 0 },
            new[] { 3, 2, 6, 7 },
        };

        var model = ShipModel.Create("Cargo", vertices, faces);
        model.MaxSpeed = 5;
        model.MaxEnergy = 10;
        return model;
    }

    private static ShipModel CreateThargoid()
    {
        // Thargoid: octagonal saucer shape
        double r = 60, h = 16;
        var cos45 = 0.7071;
        var vertices = new[]
        {
            new Vector3(r, 0, 0),           // 0
            new Vector3(r * cos45, 0, r * cos45),   // 1
            new Vector3(0, 0, r),           // 2
            new Vector3(-r * cos45, 0, r * cos45),  // 3
            new Vector3(-r, 0, 0),          // 4
            new Vector3(-r * cos45, 0, -r * cos45), // 5
            new Vector3(0, 0, -r),          // 6
            new Vector3(r * cos45, 0, -r * cos45),  // 7
            new Vector3(0, -h, 0),          // 8: top center
            new Vector3(0, h, 0),           // 9: bottom center
        };

        var faces = new List<int[]>();
        for (int i = 0; i < 8; i++)
        {
            int next = (i + 1) % 8;
            faces.Add(new[] { 8, next, i });    // top face
            faces.Add(new[] { 9, i, next });    // bottom face
        }

        var model = ShipModel.Create("Thargoid", vertices, faces.ToArray());
        model.MaxSpeed = 22;
        model.MaxEnergy = 400;
        model.MissileCount = 0;
        model.LaserPower = 5;
        model.Bounty = 500;
        return model;
    }

    private static ShipModel CreateEscapePod()
    {
        // Simple tetrahedron
        var vertices = new[]
        {
            new Vector3(0, -10, 16),
            new Vector3(-14, 10, -8),
            new Vector3(14, 10, -8),
            new Vector3(0, -6, -12),
        };

        var faces = new[]
        {
            new[] { 0, 1, 2 },
            new[] { 0, 3, 1 },
            new[] { 0, 2, 3 },
            new[] { 1, 3, 2 },
        };

        var model = ShipModel.Create("Escape Pod", vertices, faces);
        model.MaxSpeed = 8;
        model.MaxEnergy = 10;
        return model;
    }
}
