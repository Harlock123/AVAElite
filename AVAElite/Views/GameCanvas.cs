using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AVAElite.Screens;

namespace AVAElite.Views;

public class GameCanvas : Control
{
    public IGameScreen? CurrentScreen { get; set; }

    public GameCanvas()
    {
        Focusable = true;
        ClipToBounds = true;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (Bounds.Width > 0 && Bounds.Height > 0)
            CurrentScreen?.Render(context, Bounds.Width, Bounds.Height);
    }
}
