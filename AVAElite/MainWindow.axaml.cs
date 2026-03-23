using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using AVAElite.Core;
using AVAElite.Screens;
using AVAElite.Views;

namespace AVAElite;

public partial class MainWindow : Window
{
    private readonly GameState _state = new();
    private readonly InputManager _input = new();
    private GameCanvas _canvas = null!;
    private DispatcherTimer? _timer;
    private DateTime _lastTime;

    public MainWindow()
    {
        InitializeComponent();

        _canvas = this.FindControl<GameCanvas>("GameCanvas")!;

        _state.CurrentScreen = new TitleScreen(_state, OnStartGame);
        _canvas.CurrentScreen = _state.CurrentScreen;

        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;

        _lastTime = DateTime.UtcNow;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        _timer.Tick += OnTick;
        _timer.Start();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        _input.OnKeyDown(e.Key);
        e.Handled = true;
    }

    private void OnKeyUp(object? sender, KeyEventArgs e)
    {
        _input.OnKeyUp(e.Key);
        e.Handled = true;
    }

    private void OnTick(object? sender, EventArgs e)
    {
        var now = DateTime.UtcNow;
        var dt = (now - _lastTime).TotalSeconds;
        _lastTime = now;

        dt = Math.Min(dt, 0.1); // cap delta time

        _input.Update();
        _state.CurrentScreen?.Update(dt, _input);
        _canvas.CurrentScreen = _state.CurrentScreen;
        _canvas.InvalidateVisual();
    }

    private void OnStartGame()
    {
        _state.CurrentScreen = new FlightScreen(_state);
        _canvas.CurrentScreen = _state.CurrentScreen;
    }
}
