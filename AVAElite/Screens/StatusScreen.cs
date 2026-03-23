using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using AVAElite.Core;
using AVAElite.Rendering;
using AVAElite.Universe;

namespace AVAElite.Screens;

public class StatusScreen : IGameScreen
{
    private readonly GameState _state;
    private readonly StarSystem[] _galaxy;
    private readonly FlightScreen _flight;

    private static readonly IBrush GreenBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
    private static readonly IBrush DimBrush = new SolidColorBrush(Color.FromRgb(0, 150, 0));
    private static readonly IBrush YellowBrush = new SolidColorBrush(Color.FromRgb(255, 255, 0));
    private static readonly Pen BorderPen = new(new SolidColorBrush(Color.FromRgb(0, 180, 0)), 1);

    public StatusScreen(GameState state, StarSystem[] galaxy, FlightScreen flight)
    {
        _state = state;
        _galaxy = galaxy;
        _flight = flight;
    }

    public void Update(double dt, InputManager input)
    {
        if (input.JustPressed(Key.F1))
        {
            _state.Docked = false;
            _state.CurrentScreen = _flight;
        }
        if (input.JustPressed(Key.F4) && _state.Docked)
            _state.CurrentScreen = new MarketScreen(_state, _galaxy, _flight);
        if (input.JustPressed(Key.F5))
            _state.CurrentScreen = this;
        if (input.JustPressed(Key.F6))
            _state.CurrentScreen = new GalaxyMapScreen(_state, _galaxy, _flight);
        if (input.JustPressed(Key.F7))
            _state.CurrentScreen = new ShortRangeScreen(_state, _galaxy, _flight);
        if (input.JustPressed(Key.F8))
            _state.CurrentScreen = new SystemInfoScreen(_state, _galaxy, _flight);
        if (input.JustPressed(Key.F9) && _state.Docked)
            _state.CurrentScreen = new EquipmentScreen(_state, _galaxy, _flight);
    }

    public void Render(DrawingContext ctx, double w, double h)
    {
        ctx.DrawRectangle(Brushes.Black, null, new Rect(0, 0, w, h));
        ctx.DrawRectangle(null, BorderPen, new Rect(15, 15, w - 30, h - 30));

        HudRenderer.DrawTextCentered(ctx, "COMMANDER STATUS", w / 2, 25, 22, GreenBrush);

        double x = 40, y = 70, lineH = 24;
        var system = _galaxy[_state.CurrentSystem];

        DrawLine(ctx, x, ref y, lineH, "Commander:", "JAMESON");
        y += 8;
        DrawLine(ctx, x, ref y, lineH, "System:", system.Name);
        DrawLine(ctx, x, ref y, lineH, "Fuel:", $"{_state.Fuel / 10}.{_state.Fuel % 10} Light Years");
        DrawLine(ctx, x, ref y, lineH, "Cash:", $"{_state.Credits / 10}.{_state.Credits % 10} Cr");
        DrawLine(ctx, x, ref y, lineH, "Legal Status:", _state.LegalStatusText);
        DrawLine(ctx, x, ref y, lineH, "Rating:", _state.CombatRating);
        y += 8;

        // Equipment
        HudRenderer.DrawText(ctx, "EQUIPMENT:", x, y, 16, YellowBrush);
        y += lineH;

        string[] laserNames = { "None", "Pulse Laser", "Beam Laser", "Mining Laser", "Military Laser" };
        if (_state.FrontLaser > 0) { HudRenderer.DrawText(ctx, $"  Front: {laserNames[_state.FrontLaser]}", x, y, 14); y += lineH; }
        if (_state.RearLaser > 0) { HudRenderer.DrawText(ctx, $"  Rear: {laserNames[_state.RearLaser]}", x, y, 14); y += lineH; }
        if (_state.LeftLaser > 0) { HudRenderer.DrawText(ctx, $"  Left: {laserNames[_state.LeftLaser]}", x, y, 14); y += lineH; }
        if (_state.RightLaser > 0) { HudRenderer.DrawText(ctx, $"  Right: {laserNames[_state.RightLaser]}", x, y, 14); y += lineH; }
        HudRenderer.DrawText(ctx, $"  Missiles: {_state.MissileCount}", x, y, 14); y += lineH;
        if (_state.HasECM) { HudRenderer.DrawText(ctx, "  E.C.M. System", x, y, 14); y += lineH; }
        if (_state.HasFuelScoop) { HudRenderer.DrawText(ctx, "  Fuel Scoops", x, y, 14); y += lineH; }
        if (_state.HasEscapePod) { HudRenderer.DrawText(ctx, "  Escape Pod", x, y, 14); y += lineH; }
        if (_state.HasEnergyBomb) { HudRenderer.DrawText(ctx, "  Energy Bomb", x, y, 14); y += lineH; }
        if (_state.HasDockingComputer) { HudRenderer.DrawText(ctx, "  Docking Computer", x, y, 14); y += lineH; }
        if (_state.HasGalacticHyperdrive) { HudRenderer.DrawText(ctx, "  Galactic Hyperdrive", x, y, 14); y += lineH; }
        if (_state.HasLargeCargoBay) { HudRenderer.DrawText(ctx, "  Large Cargo Bay", x, y, 14); y += lineH; }

        // Cargo
        y += 12;
        HudRenderer.DrawText(ctx, "CARGO:", w / 2 + 20, 70, 16, YellowBrush);
        double cargoY = 94;
        int totalCargo = 0;
        for (int i = 0; i < 17; i++)
        {
            if (_state.Cargo[i] > 0)
            {
                HudRenderer.DrawText(ctx, $"  {CommodityTable.Items[i].Name}: {_state.Cargo[i]}{CommodityTable.Items[i].Unit}",
                    w / 2 + 20, cargoY, 14);
                cargoY += lineH;
                totalCargo += _state.Cargo[i];
            }
        }
        HudRenderer.DrawText(ctx, $"  Cargo: {totalCargo}/{_state.CargoCapacity}t", w / 2 + 20, cargoY + 8, 14, DimBrush);

        // Navigation hint
        HudRenderer.DrawTextCentered(ctx, _state.Docked
            ? "F1=Launch  F4=Market  F6=Chart  F9=Equip"
            : "F1=Flight  F6=Chart  F7=Local  F8=Data",
            w / 2, h - 50, 12, DimBrush);
    }

    private void DrawLine(DrawingContext ctx, double x, ref double y, double lineH, string label, string value)
    {
        HudRenderer.DrawText(ctx, label, x, y, 16, DimBrush);
        HudRenderer.DrawText(ctx, value, x + 200, y, 16, GreenBrush);
        y += lineH;
    }
}
