using AVAElite.Core;

namespace AVAElite.Universe;

public static class GalaxyGenerator
{
    // Digram pairs used for name generation (from original Elite)
    private static readonly string Pairs =
        "..LEXEGEZACEBISOUSESARMAABORINDIREA.ERATENBERALAVETIEDORQUANTEISRION";

    public static StarSystem[] Generate(int galaxyNumber)
    {
        var seed = EliteSeed.Galaxy1;
        for (int g = 0; g < galaxyNumber; g++)
            seed.NextGalaxy();

        var systems = new StarSystem[256];
        for (int i = 0; i < 256; i++)
        {
            systems[i] = GenerateSystem(ref seed, i);
        }
        return systems;
    }

    private static StarSystem GenerateSystem(ref EliteSeed seed, int index)
    {
        // Capture the seed state for this system
        int x = seed.Hi1;
        int y = seed.Hi0 / 2;

        int govType = (seed.Lo1 >> 3) & 7;
        int economy = seed.Hi0 & 7;
        if (govType <= 1)
            economy = economy | 2;

        int techLevel = ((economy ^ 7) + (seed.Lo1 & 3)) + (govType >> 1);
        if ((govType & 1) == 1)
            techLevel++;
        techLevel = System.Math.Clamp(techLevel, 1, 15);

        int population = 4 * techLevel + economy + govType + 1;
        int productivity = ((economy ^ 7) + 3) * (govType + 4) * population * 8;
        int radius = ((seed.Hi2 & 0x0F) + 11) * 256 + seed.Lo0;

        string name = GenerateName(ref seed);

        // 4 twists per system to advance to next
        seed.Twist();
        seed.Twist();
        seed.Twist();
        seed.Twist();

        return new StarSystem
        {
            Index = index,
            Name = name,
            X = x,
            Y = y,
            Economy = economy,
            Government = govType,
            TechLevel = techLevel,
            Population = population,
            Productivity = productivity,
            Radius = radius,
            Description = GenerateDescription(name, govType, economy),
        };
    }

    private static string GenerateName(ref EliteSeed seed)
    {
        // Save seed state for name generation
        var nameSeed = seed;
        int pairs = ((nameSeed.Hi2 & 0x40) != 0) ? 4 : 3;
        var name = new System.Text.StringBuilder();

        for (int i = 0; i < pairs; i++)
        {
            int idx = ((nameSeed.Hi2 >> 1) & 0x1F) * 2;
            nameSeed.Twist();

            if (idx < Pairs.Length - 1)
            {
                char c1 = Pairs[idx];
                char c2 = Pairs[idx + 1];
                if (c1 != '.') name.Append(c1);
                if (c2 != '.') name.Append(c2);
            }
        }

        if (name.Length == 0) return "Unknown";

        // Capitalize first letter, lowercase rest
        var result = name.ToString().ToLower();
        return char.ToUpper(result[0]) + result[1..];
    }

    private static string GenerateDescription(string name, int gov, int economy)
    {
        string[] adjectives = { "fabled", "notable", "well known", "famous", "renowned" };
        string[] features = { "its ancient forests", "its great deserts", "its vast oceans",
                              "its unusual fauna", "its exotic cuisine" };

        int hash = 0;
        foreach (char c in name) hash = hash * 31 + c;
        hash = System.Math.Abs(hash);

        string adj = adjectives[hash % adjectives.Length];
        string feat = features[(hash / 5) % features.Length];

        return $"{name} is {adj} for {feat}.";
    }
}
