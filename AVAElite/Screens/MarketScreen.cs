using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using AVAElite.Core;
using AVAElite.Rendering;
using AVAElite.Universe;

namespace AVAElite.Screens;

public class MarketScreen : IGameScreen
{
    private readonly GameState _state;
    private readonly StarSystem[] _galaxy;
    private readonly FlightScreen _flight;
    private readonly Market _market;
    private int _cursor;

    private static readonly Pen BorderPen = new(new SolidColorBrush(Color.FromRgb(0, 180, 0)), 1);
    private static readonly IBrush GreenBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
    private static readonly IBrush DimBrush = new SolidColorBrush(Color.FromRgb(0, 120, 0));
    private static readonly IBrush YellowBrush = new SolidColorBrush(Color.FromRgb(255, 255, 0));
    private static readonly IBrush RedBrush = new SolidColorBrush(Color.FromRgb(255, 80, 80));

    public MarketScreen(GameState state, StarSystem[] galaxy, FlightScreen flight)
    {
        _state = state;
        _galaxy = galaxy;
        _flight = flight;
        _market = Market.Generate(galaxy[state.CurrentSystem]);
    }

    public void Update(double dt, InputManager input)
    {
        if (input.JustPressed(Key.Up)) _cursor = Math.Max(0, _cursor - 1);
        if (input.JustPressed(Key.Down)) _cursor = Math.Min(16, _cursor + 1);

        // Buy
        if (input.JustPressed(Key.Right) || input.JustPressed(Key.Enter))
        {
            TryBuy(_cursor);
        }

        // Sell
        if (input.JustPressed(Key.Left) || input.JustPressed(Key.Back))
        {
            TrySell(_cursor);
        }

        if (input.JustPressed(Key.F1))
        {
            _state.Docked = false;
            _state.CurrentScreen = _flight;
        }
        if (input.JustPressed(Key.F5))
            _state.CurrentScreen = new StatusScreen(_state, _galaxy, _flight);
        if (input.JustPressed(Key.F6))
            _state.CurrentScreen = new GalaxyMapScreen(_state, _galaxy, _flight);
        if (input.JustPressed(Key.F9) && _state.Docked)
            _state.CurrentScreen = new EquipmentScreen(_state, _galaxy, _flight);
    }

    private void TryBuy(int item)
    {
        if (_market.Quantities[item] <= 0) return;
        if (_state.Credits < _market.Prices[item]) return;

        int totalCargo = 0;
        for (int i = 0; i < 17; i++) totalCargo += _state.Cargo[i];
        if (totalCargo >= _state.CargoCapacity) return;

        _state.Credits -= _market.Prices[item];
        _state.Cargo[item]++;
        _market.Quantities[item]--;
    }

    private void TrySell(int item)
    {
        if (_state.Cargo[item] <= 0) return;

        _state.Credits += _market.Prices[item];
        _state.Cargo[item]--;
        _market.Quantities[item]++;
    }

    public void Render(DrawingContext ctx, double w, double h)
    {
        ctx.DrawRectangle(Brushes.Black, null, new Rect(0, 0, w, h));
        ctx.DrawRectangle(null, BorderPen, new Rect(15, 15, w - 30, h - 30));

        var sys = _galaxy[_state.CurrentSystem];
        HudRenderer.DrawTextCentered(ctx, $"{sys.Name} MARKET PRICES", w / 2, 25, 20, GreenBrush);

        // Header
        double x = 40, y = 60;
        HudRenderer.DrawText(ctx, "COMMODITY", x, y, 13, DimBrush);
        HudRenderer.DrawText(ctx, "PRICE", x + 200, y, 13, DimBrush);
        HudRenderer.DrawText(ctx, "AVAIL", x + 310, y, 13, DimBrush);
        HudRenderer.DrawText(ctx, "HELD", x + 410, y, 13, DimBrush);
        HudRenderer.DrawText(ctx, "UNIT", x + 500, y, 13, DimBrush);
        y += 22;

        // Items
        for (int i = 0; i < CommodityTable.Items.Length; i++)
        {
            var c = CommodityTable.Items[i];
            bool selected = i == _cursor;
            IBrush brush = selected ? YellowBrush : GreenBrush;

            if (selected)
            {
                ctx.DrawRectangle(new SolidColorBrush(Color.FromArgb(40, 0, 255, 0)), null,
                    new Rect(x - 5, y - 2, w - 80, 20));
            }

            HudRenderer.DrawText(ctx, c.Name, x, y, 14, brush);
            HudRenderer.DrawText(ctx, _market.FormatPrice(i), x + 200, y, 14, brush);
            HudRenderer.DrawText(ctx, $"{_market.Quantities[i]}", x + 310, y, 14, brush);
            HudRenderer.DrawText(ctx, $"{_state.Cargo[i]}", x + 410, y, 14,
                _state.Cargo[i] > 0 ? YellowBrush : brush);
            HudRenderer.DrawText(ctx, c.Unit, x + 500, y, 14, brush);

            // Contraband indicator
            if (i == 3 || i == 6 || i == 10) // Slaves, Narcotics, Firearms
                HudRenderer.DrawText(ctx, "*", x + 550, y, 14, RedBrush);

            y += 22;
        }

        // Footer
        y += 10;
        int totalCargo = 0;
        for (int i = 0; i < 17; i++) totalCargo += _state.Cargo[i];

        HudRenderer.DrawText(ctx, $"Cash: {_state.Credits / 10}.{_state.Credits % 10} Cr", x, y, 16, GreenBrush);
        HudRenderer.DrawText(ctx, $"Cargo: {totalCargo}/{_state.CargoCapacity}t", x + 300, y, 16, GreenBrush);

        HudRenderer.DrawTextCentered(ctx, "Up/Down=Select  Right=Buy  Left=Sell  *=Illegal  F1=Launch  F5=Status  F9=Equip",
            w / 2, h - 40, 11, DimBrush);
    }
}
