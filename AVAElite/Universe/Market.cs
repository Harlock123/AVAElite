using System;

namespace AVAElite.Universe;

public class Market
{
    public int[] Prices { get; } = new int[17];     // in tenths of credits
    public int[] Quantities { get; } = new int[17]; // available units

    public static Market Generate(StarSystem system)
    {
        var market = new Market();
        var rng = new Random(system.X * 256 + system.Y + system.Economy * 7);

        for (int i = 0; i < CommodityTable.Items.Length; i++)
        {
            var c = CommodityTable.Items[i];
            int rand = rng.Next(256);

            int price = c.BasePrice + (rand & c.Mask) + system.Economy * c.EconFactor;
            price = Math.Max(price, 1);
            market.Prices[i] = price * 4;

            int qty = c.BaseQty + (rand & c.Mask) - system.Economy * c.EconFactor;
            qty = Math.Clamp(qty & 63, 0, 63);
            market.Quantities[i] = qty;
        }

        return market;
    }

    public string FormatPrice(int index)
    {
        int p = Prices[index];
        return $"{p / 10}.{p % 10}";
    }
}
