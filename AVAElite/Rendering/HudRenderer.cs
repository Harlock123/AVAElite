using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using AVAElite.Core;

namespace AVAElite.Rendering;

public static class HudRenderer
{
    private static readonly Pen GreenPen = new(new SolidColorBrush(Color.FromRgb(0, 255, 0)), 1);
    private static readonly Pen DimGreenPen = new(new SolidColorBrush(Color.FromRgb(0, 120, 0)), 1);
    private static readonly Pen RedPen = new(new SolidColorBrush(Color.FromRgb(255, 0, 0)), 1);
    private static readonly Pen YellowPen = new(new SolidColorBrush(Color.FromRgb(255, 255, 0)), 1);
    private static readonly IBrush GreenBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
    private static readonly IBrush RedBrush = new SolidColorBrush(Color.FromRgb(255, 60, 60));
    private static readonly IBrush YellowBrush = new SolidColorBrush(Color.FromRgb(255, 255, 0));
    private static readonly IBrush DimBrush = new SolidColorBrush(Color.FromRgb(0, 120, 0));
    private static readonly Typeface MonoTypeface = new("Courier New", FontStyle.Normal, FontWeight.Bold);

    public static void DrawFlightHud(DrawingContext ctx, double w, double h,
        double speed, double maxSpeed, double energy, double maxEnergy,
        double frontShield, double rearShield,
        int missiles, int maxMissiles, bool missileArmed,
        double altitude, double cabinTemp, double laserTemp,
        int viewDirection, string systemName)
    {
        // Main viewport border
        double viewTop = 30;
        double viewBottom = h - 150;
        double viewLeft = 10;
        double viewRight = w - 10;

        ctx.DrawRectangle(null, GreenPen, new Rect(viewLeft, viewTop, viewRight - viewLeft, viewBottom - viewTop));

        // Crosshair
        double cx = w / 2, cy = (viewTop + viewBottom) / 2;
        double cs = 12;
        ctx.DrawLine(GreenPen, new Point(cx - cs, cy), new Point(cx - 4, cy));
        ctx.DrawLine(GreenPen, new Point(cx + 4, cy), new Point(cx + cs, cy));
        ctx.DrawLine(GreenPen, new Point(cx, cy - cs), new Point(cx, cy - 4));
        ctx.DrawLine(GreenPen, new Point(cx, cy + 4), new Point(cx, cy + cs));

        // View direction label
        string[] viewLabels = { "Front View", "Rear View", "Left View", "Right View" };
        var viewLabel = new FormattedText(viewLabels[viewDirection],
            System.Globalization.CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight, MonoTypeface, 14, GreenBrush);
        ctx.DrawText(viewLabel, new Point((w - viewLabel.Width) / 2, 8));

        // Bottom panel
        double panelTop = viewBottom + 5;
        ctx.DrawRectangle(null, GreenPen, new Rect(viewLeft, panelTop, viewRight - viewLeft, h - panelTop - 10));

        // Scanner (elliptical radar)
        double scanCX = w / 2;
        double scanCY = panelTop + 55;
        double scanRX = 120;
        double scanRY = 40;

        // Scanner ellipse
        var scanGeom = new EllipseGeometry(new Rect(scanCX - scanRX, scanCY - scanRY, scanRX * 2, scanRY * 2));
        ctx.DrawGeometry(null, DimGreenPen, scanGeom);
        // Scanner cross
        ctx.DrawLine(DimGreenPen, new Point(scanCX - scanRX, scanCY), new Point(scanCX + scanRX, scanCY));
        ctx.DrawLine(DimGreenPen, new Point(scanCX, scanCY - scanRY), new Point(scanCX, scanCY + scanRY));

        // Speed bar (left side of panel)
        double barX = viewLeft + 20;
        double barY = panelTop + 10;
        DrawBar(ctx, barX, barY, "SP", speed / maxSpeed, GreenPen, GreenBrush);

        // Energy bars (right side)
        double eBarX = viewRight - 170;
        DrawBar(ctx, eBarX, barY, "FH", frontShield / maxEnergy, GreenPen, GreenBrush);
        DrawBar(ctx, eBarX, barY + 16, "AH", rearShield / maxEnergy, GreenPen, GreenBrush);
        DrawBar(ctx, eBarX, barY + 32, "EN", energy / maxEnergy, GreenPen, GreenBrush);
        DrawBar(ctx, eBarX, barY + 48, "CT", cabinTemp, YellowPen, YellowBrush);
        DrawBar(ctx, eBarX, barY + 64, "LT", laserTemp, RedPen, RedBrush);

        // Missiles
        double missileX = viewLeft + 20;
        double missileY = panelTop + 90;
        for (int i = 0; i < maxMissiles; i++)
        {
            var b = i < missiles ? (missileArmed && i == missiles - 1 ? RedBrush : GreenBrush) : DimBrush;
            var rect = new Rect(missileX + i * 18, missileY, 14, 8);
            ctx.DrawRectangle(b, null, rect);
        }

        // System name
        var sysText = new FormattedText(systemName,
            System.Globalization.CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight, MonoTypeface, 12, GreenBrush);
        ctx.DrawText(sysText, new Point(viewRight - sysText.Width - 20, panelTop + 92));
    }

    private static void DrawBar(DrawingContext ctx, double x, double y,
        string label, double fraction, Pen pen, IBrush brush)
    {
        fraction = Math.Clamp(fraction, 0, 1);
        double barWidth = 120;
        double barHeight = 10;

        var labelText = new FormattedText(label,
            System.Globalization.CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight, MonoTypeface, 10, brush);
        ctx.DrawText(labelText, new Point(x, y - 1));

        double bx = x + 25;
        ctx.DrawRectangle(null, pen, new Rect(bx, y, barWidth, barHeight));
        if (fraction > 0)
            ctx.DrawRectangle(brush, null, new Rect(bx + 1, y + 1, (barWidth - 2) * fraction, barHeight - 2));
    }

    public static void DrawScannerBlip(DrawingContext ctx, double scanCX, double scanCY,
        double scanRX, double scanRY, double relX, double relY, double relZ, bool hostile)
    {
        // relX = left/right, relY = up/down (altitude), relZ = front/back
        double bx = scanCX + relX * scanRX;
        double bz = scanCY - relZ * scanRY;
        double stickLen = relY * scanRY * 0.5;

        var pen = hostile ? RedPen : YellowPen;

        // Vertical stick showing altitude difference
        ctx.DrawLine(pen, new Point(bx, bz), new Point(bx, bz - stickLen));
        // Blip dot at the end of the stick
        ctx.DrawLine(pen, new Point(bx - 2, bz - stickLen), new Point(bx + 2, bz - stickLen));
    }

    public static void DrawCompass(DrawingContext ctx, double cx, double cy, double radius,
        Vector3 directionToTarget)
    {
        ctx.DrawGeometry(null, DimGreenPen, new EllipseGeometry(new Rect(cx - radius, cy - radius, radius * 2, radius * 2)));

        var dir = directionToTarget.Normalized();
        double dotX = cx + dir.X * radius * 0.8;
        double dotY = cy - dir.Y * radius * 0.8;
        bool inFront = dir.Z > 0;

        var pen = inFront ? GreenPen : RedPen;
        ctx.DrawEllipse(inFront ? GreenBrush : RedBrush, pen,
            new Point(dotX, dotY), 3, 3);
    }

    public static void DrawText(DrawingContext ctx, string text, double x, double y,
        double size = 14, IBrush? brush = null)
    {
        var ft = new FormattedText(text,
            System.Globalization.CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight, MonoTypeface, size, brush ?? GreenBrush);
        ctx.DrawText(ft, new Point(x, y));
    }

    public static void DrawTextCentered(DrawingContext ctx, string text, double centerX, double y,
        double size = 14, IBrush? brush = null)
    {
        var ft = new FormattedText(text,
            System.Globalization.CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight, MonoTypeface, size, brush ?? GreenBrush);
        ctx.DrawText(ft, new Point(centerX - ft.Width / 2, y));
    }
}
