using System;
using Avalonia;
using Avalonia.Media;
using AVAElite.Core;
using AVAElite.Models;
using AVAElite.Rendering;

namespace AVAElite.Screens;

public class TitleScreen : IGameScreen
{
    private readonly GameState _state;
    private readonly Action _onStart;
    private double _rotation;
    private double _rotationX;
    private readonly double[] _starX;
    private readonly double[] _starY;

    private static readonly Pen WireframePen = new(new SolidColorBrush(Color.FromRgb(0, 255, 0)), 1.5);
    private static readonly Pen BorderPen = new(new SolidColorBrush(Color.FromRgb(0, 180, 0)), 1);
    private static readonly IBrush TextBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
    private static readonly IBrush DimBrush = new SolidColorBrush(Color.FromRgb(0, 180, 0));
    private static readonly IBrush StarBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200));
    private static readonly Typeface MonoTypeface = new("Courier New", FontStyle.Normal, FontWeight.Bold);

    public TitleScreen(GameState state, Action onStart)
    {
        _state = state;
        _onStart = onStart;

        var rng = new Random(42);
        _starX = new double[80];
        _starY = new double[80];
        for (int i = 0; i < _starX.Length; i++)
        {
            _starX[i] = rng.NextDouble() * 1200;
            _starY[i] = rng.NextDouble() * 800;
        }
    }

    public void Update(double deltaTime, InputManager input)
    {
        _rotation += deltaTime * 0.6;
        _rotationX += deltaTime * 0.15;

        if (input.JustPressed(Avalonia.Input.Key.Space))
        {
            _onStart();
        }
    }

    public void Render(DrawingContext ctx, double width, double height)
    {
        // Black background
        ctx.DrawRectangle(Brushes.Black, null, new Rect(0, 0, width, height));

        // Stars
        Renderer3D.DrawStarfield(ctx, width, height, _starX, _starY, StarBrush);

        // Border
        double bx = 20, by = 20;
        ctx.DrawRectangle(null, BorderPen, new Rect(bx, by, width - 2 * bx, height - 2 * by));

        // Title text
        var titleText = new FormattedText(
            "--- E L I T E ---",
            System.Globalization.CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            MonoTypeface,
            36,
            TextBrush);
        ctx.DrawText(titleText, new Point((width - titleText.Width) / 2, 50));

        // Rotating Cobra
        var orientation = Matrix3x3.RotationY(_rotation) * Matrix3x3.RotationX(_rotationX);
        var shipPos = new Vector3(0, 0, 350);
        var cameraPos = Vector3.Zero;
        var cameraOri = Matrix3x3.Identity;

        Renderer3D.DrawShip(ctx, ShipDefinitions.CobraMkIII, shipPos, orientation,
            cameraPos, cameraOri, width / 2, height / 2, WireframePen);

        // Subtitle
        var subText = new FormattedText(
            "COMMANDER JAMESON",
            System.Globalization.CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            MonoTypeface,
            18,
            DimBrush);
        ctx.DrawText(subText, new Point((width - subText.Width) / 2, height - 130));

        // Prompt
        var blink = ((int)(_rotation * 3)) % 2 == 0;
        if (blink)
        {
            var promptText = new FormattedText(
                "Press SPACE to begin",
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                MonoTypeface,
                20,
                TextBrush);
            ctx.DrawText(promptText, new Point((width - promptText.Width) / 2, height - 80));
        }

        // Credits line
        var creditText = new FormattedText(
            "(C) Acornsoft 1984    Recreated in Avalonia",
            System.Globalization.CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            MonoTypeface,
            12,
            DimBrush);
        ctx.DrawText(creditText, new Point((width - creditText.Width) / 2, height - 45));
    }
}
