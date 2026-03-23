namespace AVAElite.Universe;

public class Commodity
{
    public string Name { get; init; } = "";
    public string Unit { get; init; } = "t";
    public int BasePrice { get; init; }   // in tenths of credit
    public int EconFactor { get; init; }
    public int BaseQty { get; init; }
    public int Mask { get; init; }
}

public static class CommodityTable
{
    public static readonly Commodity[] Items =
    {
        new() { Name = "Food",         BasePrice = 19,  EconFactor = -2,  BaseQty = 6,   Mask = 0x01, Unit = "t" },
        new() { Name = "Textiles",     BasePrice = 20,  EconFactor = -1,  BaseQty = 10,  Mask = 0x03, Unit = "t" },
        new() { Name = "Radioactives", BasePrice = 65,  EconFactor = -3,  BaseQty = 2,   Mask = 0x07, Unit = "t" },
        new() { Name = "Slaves",       BasePrice = 40,  EconFactor = -5,  BaseQty = 226, Mask = 0x1F, Unit = "t" },
        new() { Name = "Liquor/Wines", BasePrice = 83,  EconFactor = -5,  BaseQty = 251, Mask = 0x0F, Unit = "t" },
        new() { Name = "Luxuries",     BasePrice = 196, EconFactor = 8,   BaseQty = 54,  Mask = 0x03, Unit = "t" },
        new() { Name = "Narcotics",    BasePrice = 235, EconFactor = 29,  BaseQty = 8,   Mask = 0x78, Unit = "t" },
        new() { Name = "Computers",    BasePrice = 154, EconFactor = 14,  BaseQty = 56,  Mask = 0x03, Unit = "t" },
        new() { Name = "Machinery",    BasePrice = 117, EconFactor = 6,   BaseQty = 40,  Mask = 0x07, Unit = "t" },
        new() { Name = "Alloys",       BasePrice = 78,  EconFactor = 1,   BaseQty = 17,  Mask = 0x1F, Unit = "t" },
        new() { Name = "Firearms",     BasePrice = 124, EconFactor = 13,  BaseQty = 29,  Mask = 0x07, Unit = "t" },
        new() { Name = "Furs",         BasePrice = 176, EconFactor = -9,  BaseQty = 220, Mask = 0x3F, Unit = "t" },
        new() { Name = "Minerals",     BasePrice = 32,  EconFactor = -1,  BaseQty = 53,  Mask = 0x03, Unit = "t" },
        new() { Name = "Gold",         BasePrice = 97,  EconFactor = -1,  BaseQty = 66,  Mask = 0x07, Unit = "kg" },
        new() { Name = "Platinum",     BasePrice = 171, EconFactor = -2,  BaseQty = 55,  Mask = 0x1F, Unit = "kg" },
        new() { Name = "Gem-Stones",   BasePrice = 45,  EconFactor = -1,  BaseQty = 250, Mask = 0x0F, Unit = "g" },
        new() { Name = "Alien Items",  BasePrice = 53,  EconFactor = 15,  BaseQty = 192, Mask = 0x07, Unit = "t" },
    };
}
