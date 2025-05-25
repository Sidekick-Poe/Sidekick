namespace Sidekick.Common.Platform.EventArgs;

/// <summary>
/// Represents the event arguments for a drag event within <see cref="IKeyboardProvider"/>.
/// </summary>
public class DraggedEventArgs(int x, int y)
{
    public int X { get; init; } = x;
    public int Y { get; init; } = y;
}
