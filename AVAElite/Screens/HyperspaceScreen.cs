using System;
using Avalonia;
using Avalonia.Media;
using AVAElite.Core;
using AVAElite.Rendering;

namespace AVAElite.Screens;

public class HyperspaceScreen : IGameScreen
{
    private readonly GameState _state;
    private readonly Action _onComplete;
    private readonly string _targetName;
    private double _timer;
    private readonly double _duration = 3.0;

    private static readonly IBrush GreenBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));

    public HyperspaceScreen(GameState state, string targetName, Action onComplete)
    {
        _state = state;
        _targetName = targetName;
        _onComplete = onComplete;
    }

    public void Update(double dt, InputManager input)
    {
        _timer += dt;
        if (_timer >= _duration)
            _onComplete();
    }

    public void Render(DrawingContext ctx, double w, double h)
    {
        ctx.DrawRectangle(Brushes.Black, null, new Rect(0, 0, w, h));

        double progress = Math.Min(_timer / _duration, 1.0);

        // Hyperspace tunnel effect: expanding rings
        double cx = w / 2, cy = h / 2;
        var pen = new Pen(new SolidColorBrush(Color.FromArgb(
            (byte)(255 * (1 - progress * 0.5)), 0, 255, 0)), 1.5);

        for (int i = 0; i < 12; i++)
        {
            double phase = (progress * 4 + i * 0.3) % 1.0;
            double radius = phase * Math.Min(w, h) * 0.6;
            if (radius > 1)
            {
                byte alpha = (byte)(255 * (1 - phase));
                var ringPen = new Pen(new SolidColorBrush(Color.FromArgb(alpha, 0, 255, 0)), 1);
                ctx.DrawEllipse(null, ringPen, new Point(cx, cy), radius, radius * 0.4);
            }
        }

        // Streaking stars
        var starPen = new Pen(Brushes.White, 1);
        var rng = new Random(42);
        for (int i = 0; i < 40; i++)
        {
            double angle = rng.NextDouble() * Math.PI * 2;
            double startR = rng.NextDouble() * 50 + progress * 300;
            double endR = startR + 50 + progress * 200;

            double x1 = cx + Math.Cos(angle) * startR;
            double y1 = cy + Math.Sin(angle) * startR * 0.4;
            double x2 = cx + Math.Cos(angle) * endR;
            double y2 = cy + Math.Sin(angle) * endR * 0.4;

            ctx.DrawLine(starPen, new Point(x1, y1), new Point(x2, y2));
        }

        HudRenderer.DrawTextCentered(ctx, "HYPERSPACE", cx, 40, 24, GreenBrush);
        HudRenderer.DrawTextCentered(ctx, _targetName, cx, h - 60, 20, GreenBrush);
    }
}
