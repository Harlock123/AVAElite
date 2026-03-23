using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using AVAElite.Core;
using AVAElite.Rendering;
using AVAElite.Universe;

namespace AVAElite.Screens;

public class EquipmentScreen : IGameScreen
{
    private readonly GameState _state;
    private readonly StarSystem[] _galaxy;
    private readonly FlightScreen _flight;
    private int _cursor;

    private record EquipItem(string Name, int Price, Func<GameState, bool> CanBuy, Action<GameState> Buy);

    private readonly List<EquipItem> _items;

    private static readonly Pen BorderPen = new(new SolidColorBrush(Color.FromRgb(0, 180, 0)), 1);
    private static readonly IBrush GreenBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
    private static readonly IBrush DimBrush = new SolidColorBrush(Color.FromRgb(0, 120, 0));
    private static readonly IBrush YellowBrush = new SolidColorBrush(Color.FromRgb(255, 255, 0));
    private static readonly IBrush RedBrush = new SolidColorBrush(Color.FromRgb(255, 80, 80));

    public EquipmentScreen(GameState state, StarSystem[] galaxy, FlightScreen flight)
    {
        _state = state;
        _galaxy = galaxy;
        _flight = flight;

        _items = new List<EquipItem>
        {
            new("Fuel", 20, s => s.Fuel < s.MaxFuel, s => s.Fuel = s.MaxFuel),
            new("Missile", 300, s => s.MissileCount < s.MaxMissiles, s => s.MissileCount++),
            new("Large Cargo Bay", 4000, s => !s.HasLargeCargoBay, s => s.HasLargeCargoBay = true),
            new("E.C.M. System", 6000, s => !s.HasECM, s => s.HasECM = true),
            new("Pulse Laser (front)", 4000, s => s.FrontLaser < 1, s => s.FrontLaser = 1),
            new("Pulse Laser (rear)", 4000, s => s.RearLaser < 1, s => s.RearLaser = 1),
            new("Beam Laser (front)", 10000, s => s.FrontLaser < 2, s => s.FrontLaser = 2),
            new("Beam Laser (rear)", 10000, s => s.RearLaser < 2, s => s.RearLaser = 2),
            new("Mining Laser (front)", 8000, s => s.FrontLaser < 3, s => s.FrontLaser = 3),
            new("Military Laser (front)", 60000, s => s.FrontLaser < 4, s => s.FrontLaser = 4),
            new("Military Laser (rear)", 60000, s => s.RearLaser < 4, s => s.RearLaser = 4),
            new("Fuel Scoops", 5250, s => !s.HasFuelScoop, s => s.HasFuelScoop = true),
            new("Escape Pod", 10000, s => !s.HasEscapePod, s => s.HasEscapePod = true),
            new("Energy Bomb", 9000, s => !s.HasEnergyBomb, s => s.HasEnergyBomb = true),
            new("Docking Computer", 15000, s => !s.HasDockingComputer, s => s.HasDockingComputer = true),
            new("Galactic Hyperdrive", 50000, s => !s.HasGalacticHyperdrive, s => s.HasGalacticHyperdrive = true),
        };
    }

    public void Update(double dt, InputManager input)
    {
        if (input.JustPressed(Key.Up)) _cursor = Math.Max(0, _cursor - 1);
        if (input.JustPressed(Key.Down)) _cursor = Math.Min(_items.Count - 1, _cursor + 1);

        if (input.JustPressed(Key.Enter) || input.JustPressed(Key.Right))
        {
            TryBuy(_cursor);
        }

        if (input.JustPressed(Key.F1))
        {
            _state.Docked = false;
            _state.CurrentScreen = _flight;
        }
        if (input.JustPressed(Key.F4))
            _state.CurrentScreen = new MarketScreen(_state, _galaxy, _flight);
        if (input.JustPressed(Key.F5))
            _state.CurrentScreen = new StatusScreen(_state, _galaxy, _flight);
    }

    private void TryBuy(int index)
    {
        if (index < 0 || index >= _items.Count) return;
        var item = _items[index];

        if (!item.CanBuy(_state)) return;
        if (_state.Credits < item.Price) return;

        _state.Credits -= item.Price;
        item.Buy(_state);
    }

    public void Render(DrawingContext ctx, double w, double h)
    {
        ctx.DrawRectangle(Brushes.Black, null, new Rect(0, 0, w, h));
        ctx.DrawRectangle(null, BorderPen, new Rect(15, 15, w - 30, h - 30));

        HudRenderer.DrawTextCentered(ctx, "EQUIP SHIP", w / 2, 25, 22, GreenBrush);

        double x = 50, y = 65;

        HudRenderer.DrawText(ctx, "ITEM", x, y, 13, DimBrush);
        HudRenderer.DrawText(ctx, "PRICE", x + 400, y, 13, DimBrush);
        y += 22;

        for (int i = 0; i < _items.Count; i++)
        {
            var item = _items[i];
            bool selected = i == _cursor;
            bool canBuy = item.CanBuy(_state) && _state.Credits >= item.Price;

            IBrush brush = selected ? YellowBrush : (canBuy ? GreenBrush : DimBrush);

            if (selected)
            {
                ctx.DrawRectangle(new SolidColorBrush(Color.FromArgb(40, 0, 255, 0)), null,
                    new Rect(x - 5, y - 2, w - 100, 20));
            }

            HudRenderer.DrawText(ctx, item.Name, x, y, 14, brush);
            HudRenderer.DrawText(ctx, $"{item.Price / 10}.{item.Price % 10} Cr", x + 400, y, 14, brush);

            if (!item.CanBuy(_state))
                HudRenderer.DrawText(ctx, "OWNED", x + 550, y, 12, DimBrush);

            y += 22;
        }

        y += 15;
        HudRenderer.DrawText(ctx, $"Cash: {_state.Credits / 10}.{_state.Credits % 10} Cr", x, y, 16, GreenBrush);

        HudRenderer.DrawTextCentered(ctx, "Up/Down=Select  Enter=Buy  F1=Launch  F4=Market  F5=Status",
            w / 2, h - 40, 12, DimBrush);
    }
}
