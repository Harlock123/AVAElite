using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using AVAElite.Core;
using AVAElite.Rendering;
using AVAElite.Universe;

namespace AVAElite.Screens;

public class GalaxyMapScreen : IGameScreen
{
    private readonly GameState _state;
    private readonly StarSystem[] _galaxy;
    private readonly FlightScreen _flight;

    private static readonly Pen GreenPen = new(new SolidColorBrush(Color.FromRgb(0, 255, 0)), 1);
    private static readonly Pen DimPen = new(new SolidColorBrush(Color.FromRgb(0, 100, 0)), 0.5);
    private static readonly Pen YellowPen = new(new SolidColorBrush(Color.FromRgb(255, 255, 0)), 1);
    private static readonly Pen BorderPen = new(new SolidColorBrush(Color.FromRgb(0, 180, 0)), 1);
    private static readonly IBrush GreenBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
    private static readonly IBrush YellowBrush = new SolidColorBrush(Color.FromRgb(255, 255, 0));
    private static readonly IBrush DimBrush = new SolidColorBrush(Color.FromRgb(0, 120, 0));

    public GalaxyMapScreen(GameState state, StarSystem[] galaxy, FlightScreen flight)
    {
        _state = state;
        _galaxy = galaxy;
        _flight = flight;
    }

    public void Update(double dt, InputManager input)
    {
        // Navigate with cursor keys
        var current = _galaxy[_state.SelectedSystem];
        StarSystem? nearest = null;
        double bestDist = double.MaxValue;

        if (input.JustPressed(Key.Left) || input.JustPressed(Key.Right) ||
            input.JustPressed(Key.Up) || input.JustPressed(Key.Down))
        {
            double dx = 0, dy = 0;
            if (input.JustPressed(Key.Left)) dx = -1;
            if (input.JustPressed(Key.Right)) dx = 1;
            if (input.JustPressed(Key.Up)) dy = -1;
            if (input.JustPressed(Key.Down)) dy = 1;

            foreach (var sys in _galaxy)
            {
                if (sys.Index == _state.SelectedSystem) continue;
                double sdx = sys.X - current.X;
                double sdy = sys.Y - current.Y;

                // Only consider systems in the pressed direction
                if (dx != 0 && Math.Sign(sdx) != Math.Sign(dx)) continue;
                if (dy != 0 && Math.Sign(sdy) != Math.Sign(dy)) continue;

                double dist = Math.Sqrt(sdx * sdx + sdy * sdy);
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
        {
            _state.CurrentScreen = _flight;
        }
        if (input.JustPressed(Key.F5))
            _state.CurrentScreen = new StatusScreen(_state, _galaxy, _flight);
        if (input.JustPressed(Key.F7))
            _state.CurrentScreen = new ShortRangeScreen(_state, _galaxy, _flight);
        if (input.JustPressed(Key.F8))
            _state.CurrentScreen = new SystemInfoScreen(_state, _galaxy, _flight);
    }

    public void Render(DrawingContext ctx, double w, double h)
    {
        ctx.DrawRectangle(Brushes.Black, null, new Rect(0, 0, w, h));
        ctx.DrawRectangle(null, BorderPen, new Rect(15, 15, w - 30, h - 30));

        HudRenderer.DrawTextCentered(ctx, $"GALACTIC CHART {_state.CurrentGalaxy + 1}", w / 2, 25, 20, GreenBrush);

        double mapX = 30, mapY = 55;
        double mapW = w - 60, mapH = h - 130;
        double scaleX = mapW / 256.0;
        double scaleY = mapH / 128.0;

        // Draw all systems as dots
        foreach (var sys in _galaxy)
        {
            double sx = mapX + sys.X * scaleX;
            double sy = mapY + sys.Y * scaleY;
            double dotSize = 1 + sys.TechLevel * 0.15;

            bool isCurrent = sys.Index == _state.CurrentSystem;
            bool isSelected = sys.Index == _state.SelectedSystem;

            if (isSelected)
            {
                ctx.DrawEllipse(null, YellowPen, new Point(sx, sy), dotSize + 4, dotSize + 4);
                HudRenderer.DrawText(ctx, sys.Name, sx + 8, sy - 8, 12, YellowBrush);
            }
            else if (isCurrent)
            {
                ctx.DrawEllipse(GreenBrush, GreenPen, new Point(sx, sy), dotSize + 2, dotSize + 2);
            }
            else
            {
                ctx.DrawEllipse(GreenBrush, null, new Point(sx, sy), dotSize, dotSize);
            }
        }

        // Info line
        var selected = _galaxy[_state.SelectedSystem];
        var currentSys = _galaxy[_state.CurrentSystem];
        double dist = currentSys.DistanceTo(selected);

        HudRenderer.DrawText(ctx, $"{selected.Name}    Distance: {dist:F1} LY    Fuel: {_state.Fuel / 10}.{_state.Fuel % 10} LY",
            mapX, h - 60, 14, GreenBrush);

        HudRenderer.DrawTextCentered(ctx, "Arrows=Select  F1=Flight  F7=Local  F8=Data", w / 2, h - 35, 12, DimBrush);
    }
}
