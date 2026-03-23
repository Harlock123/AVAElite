using System.Collections.Generic;
using Avalonia.Input;

namespace AVAElite.Core;

public class InputManager
{
    private readonly HashSet<Key> _pressed = new();
    private readonly HashSet<Key> _justPressed = new();
    private readonly HashSet<Key> _previousFrame = new();

    public void OnKeyDown(Key key)
    {
        _pressed.Add(key);
    }

    public void OnKeyUp(Key key)
    {
        _pressed.Remove(key);
    }

    public void Update()
    {
        _justPressed.Clear();
        foreach (var key in _pressed)
        {
            if (!_previousFrame.Contains(key))
                _justPressed.Add(key);
        }
        _previousFrame.Clear();
        foreach (var key in _pressed)
            _previousFrame.Add(key);
    }

    public bool IsDown(Key key) => _pressed.Contains(key);
    public bool JustPressed(Key key) => _justPressed.Contains(key);
}
