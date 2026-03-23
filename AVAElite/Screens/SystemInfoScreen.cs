using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using AVAElite.Core;
using AVAElite.Rendering;
using AVAElite.Universe;

namespace AVAElite.Screens;

public class SystemInfoScreen : IGameScreen
{
    private readonly GameState _state;
    private readonly StarSystem[] _galaxy;
    private readonly FlightScreen _flight;

    private static readonly Pen BorderPen = new(new SolidColorBrush(Color.FromRgb(0, 180, 0)), 1);
    private static readonly IBrush GreenBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
    private static readonly IBrush DimBrush = new SolidColorBrush(Color.FromRgb(0, 120, 0));
    private static readonly IBrush YellowBrush = new SolidColorBrush(Color.FromRgb(255, 255, 0));

    public SystemInfoScreen(GameState state, StarSystem[] galaxy, FlightScreen flight)
    {
        _state = state;
        _galaxy = galaxy;
        _flight = flight;
    }

    public void Update(double dt, InputManager input)
    {
        if (input.JustPressed(Key.F1))
            _state.CurrentScreen = _flight;
        if (input.JustPressed(Key.F5))
            _state.CurrentScreen = new StatusScreen(_state, _galaxy, _flight);
        if (input.JustPressed(Key.F6))
            _state.CurrentScreen = new GalaxyMapScreen(_state, _galaxy, _flight);
        if (input.JustPressed(Key.F7))
            _state.CurrentScreen = new ShortRangeScreen(_state, _galaxy, _flight);
    }

    public void Render(DrawingContext ctx, double w, double h)
    {
        ctx.DrawRectangle(Brushes.Black, null, new Rect(0, 0, w, h));
        ctx.DrawRectangle(null, BorderPen, new Rect(15, 15, w - 30, h - 30));

        var sys = _galaxy[_state.SelectedSystem];
        var current = _galaxy[_state.CurrentSystem];
        double dist = current.DistanceTo(sys);

        HudRenderer.DrawTextCentered(ctx, "SYSTEM DATA", w / 2, 25, 22, GreenBrush);

        double x = 60, y = 80, lineH = 30;

        HudRenderer.DrawText(ctx, sys.Name, x, y, 28, YellowBrush);
        y += 40;

        DrawInfo(ctx, x, ref y, lineH, "Economy:", sys.EconomyName);
        DrawInfo(ctx, x, ref y, lineH, "Government:", sys.GovernmentName);
        DrawInfo(ctx, x, ref y, lineH, "Tech Level:", sys.TechLevelString);
        DrawInfo(ctx, x, ref y, lineH, "Population:", sys.PopulationString);
        DrawInfo(ctx, x, ref y, lineH, "Productivity:", sys.ProductivityString);
        DrawInfo(ctx, x, ref y, lineH, "Radius:", $"{sys.Radius} km");
        DrawInfo(ctx, x, ref y, lineH, "Distance:", $"{dist:F1} Light Years");

        y += 20;
        HudRenderer.DrawText(ctx, sys.Description, x, y, 14, DimBrush);

        HudRenderer.DrawTextCentered(ctx, "F1=Flight  F5=Status  F6=Chart  F7=Local", w / 2, h - 40, 12, DimBrush);
    }

    private void DrawInfo(DrawingContext ctx, double x, ref double y, double lineH, string label, string value)
    {
        HudRenderer.DrawText(ctx, label, x, y, 16, DimBrush);
        HudRenderer.DrawText(ctx, value, x + 200, y, 16, GreenBrush);
        y += lineH;
    }
}
