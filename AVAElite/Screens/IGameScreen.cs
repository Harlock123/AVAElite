using Avalonia.Media;
using AVAElite.Core;

namespace AVAElite.Screens;

public interface IGameScreen
{
    void Update(double deltaTime, InputManager input);
    void Render(DrawingContext ctx, double width, double height);
}
