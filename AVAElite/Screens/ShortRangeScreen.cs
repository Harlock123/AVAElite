using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using AVAElite.Core;
using AVAElite.Rendering;
using AVAElite.Universe;

namespace AVAElite.Screens;

public class ShortRangeScreen : IGameScreen
{
    private readonly GameState _state;
    private readonly StarSystem[] _galaxy;
    private readonly FlightScreen _flight;

    private static readonly Pen GreenPen = new(new SolidColorBrush(Color.FromRgb(0, 255, 0)), 1);
    private static readonly Pen FuelCirclePen = new(new SolidColorBrush(Color.FromRgb(0, 100, 0)), 1)
        { DashStyle = DashStyle.Dash };
    private static readonly Pen YellowPen = new(new SolidColorBrush(Color.FromRgb(255, 255, 0)), 1);
    private static readonly Pen BorderPen = new(new SolidColorBrush(Color.FromRgb(0, 180, 0)), 1);
    private static readonly IBrush GreenBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
    private static readonly IBrush YellowBrush = new SolidColorBrush(Color.FromRgb(255, 255, 0));
    private static readonly IBrush DimBrush = new SolidColorBrush(Color.FromRgb(0, 120, 0));

    public ShortRangeScreen(GameState state, StarSystem[] galaxy, FlightScreen flight)
    {
        _state = state;
        _galaxy = galaxy;
        _flight = flight;
    }

    public void Update(double dt, InputManager input)
    {
        var current = _galaxy[_state.SelectedSystem];

        if (input.JustPressed(Key.Left) || input.JustPressed(Key.Right) ||
            input.JustPressed(Key.Up) || input.JustPressed(Key.Down))
        {
            double dx = 0, dy = 0;
            if (input.JustPressed(Key.Left)) dx = -1;
            if (input.JustPressed(Key.Right)) dx = 1;
            if (input.JustPressed(Key.Up)) dy = -1;
            if (input.JustPressed(Key.Down)) dy = 1;

            StarSystem? nearest = null;
            double bestDist = double.MaxValue;

            foreach (var sys in _galaxy)
            {
                if (sys.Index == _state.SelectedSystem) continue;
                double sdx = sys.X - current.X;
                double sdy = sys.Y - current.Y;
                double dist = Math.Sqrt(sdx * sdx + sdy * sdy);
                if (dist > 50) continue; // only show nearby systems

                if (dx != 0 && Math.Sign(sdx) != Math.Sign(dx)) continue;
                if (dy != 0 && Math.Sign(sdy) != Math.Sign(dy)) continue;

                if (dist < bestDist)
                {
                    bestDist = dist;
                    nearest = sys;
                }
            }

            if (nearest != null)
                _state.SelectedSystem = nearest.Index;
        }

        if (input.JustPressed(Key.F1))
            _state.CurrentScreen = _flight;
        if (input.JustPressed(Key.F5))
            _state.CurrentScreen = new StatusScreen(_state, _galaxy, _flight);
        if (input.JustPressed(Key.F6))
            _state.CurrentScreen = new GalaxyMapScreen(_state, _galaxy, _flight);
        if (input.JustPressed(Key.F8))
            _state.CurrentScreen = new SystemInfoScreen(_state, _galaxy, _flight);
    }

    public void Render(DrawingContext ctx, double w, double h)
    {
        ctx.DrawRectangle(Brushes.Black, null, new Rect(0, 0, w, h));
        ctx.DrawRectangle(null, BorderPen, new Rect(15, 15, w - 30, h - 30));

        HudRenderer.DrawTextCentered(ctx, "SHORT RANGE CHART", w / 2, 25, 20, GreenBrush);

        var currentSys = _galaxy[_state.CurrentSystem];
        double centerX = w / 2;
        double centerY = h / 2;
        double scale = 12.0; // pixels per coordinate unit

        // Fuel range circle
        double fuelRange = _state.Fuel / 10.0; // in LY
        double fuelPixels = fuelRange / 0.4 * scale;
        ctx.DrawEllipse(null, FuelCirclePen, new Point(centerX, centerY), fuelPixels, fuelPixels);

        // Draw nearby systems
        foreach (var sys in _galaxy)
        {
            double dx = sys.X - currentSys.X;
            double dy = sys.Y - currentSys.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            if (dist > 50) continue;

            double sx = centerX + dx * scale;
            double sy = centerY + dy * scale;
            double dotSize = 2 + sys.TechLevel * 0.2;

            bool isSelected = sys.Index == _state.SelectedSystem;
            bool isCurrent = sys.Index == _state.CurrentSystem;

            if (isCurrent)
            {
                ctx.DrawEllipse(GreenBrush, GreenPen, new Point(sx, sy), dotSize + 3, dotSize + 3);
                HudRenderer.DrawText(ctx, sys.Name, sx + 10, sy - 8, 13, GreenBrush);
            }
            else if (isSelected)
            {
                ctx.DrawEllipse(null, YellowPen, new Point(sx, sy), dotSize + 4, dotSize + 4);
                HudRenderer.DrawText(ctx, sys.Name, sx + 10, sy - 8, 13, YellowBrush);
            }
            else
            {
                ctx.DrawEllipse(GreenBrush, null, new Point(sx, sy), dotSize, dotSize);
                HudRenderer.DrawText(ctx, sys.Name, sx + 8, sy - 6, 10, DimBrush);
            }
        }

        // Info
        var selected = _galaxy[_state.SelectedSystem];
        double distToSel = currentSys.DistanceTo(selected);
        HudRenderer.DrawText(ctx,
            $"{selected.Name}  ({selected.EconomyName}, {selected.GovernmentName})  Dist: {distToSel:F1} LY",
            30, h - 60, 14, GreenBrush);

        HudRenderer.DrawTextCentered(ctx, "Arrows=Select  J=Hyperspace  F1=Flight  F6=Galaxy  F8=Data",
            w / 2, h - 35, 12, DimBrush);
    }
}
