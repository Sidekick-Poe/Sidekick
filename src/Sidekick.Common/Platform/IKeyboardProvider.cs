using Sidekick.Common.Initialization;
using Sidekick.Common.Platform.EventArgs;

namespace Sidekick.Common.Platform;

/// <summary>
/// Service providing keyboard functions
/// </summary>
public interface IKeyboardProvider : IInitializableService
{
    /// <summary>
    /// Event that indicates that a key was pressed
    /// </summary>
    event Action<string> OnKeyDown;

    /// <summary>
    /// Event triggered when the user scrolls down.
    /// </summary>
    event Action<ScrollEventArgs> OnScrollDown;

    /// <summary>
    /// Event triggered when the user scrolls up.
    /// </summary>
    event Action<ScrollEventArgs> OnScrollUp;

    /// <summary>
    /// Event triggered when the user drags the mouse.
    /// </summary>
    event Action<DraggedEventArgs>? OnMouseDrag;

    /// <summary>
    /// Register keyboard hooks to capture keybinds.
    /// </summary>
    void RegisterHooks();

    /// <summary>
    /// Command to send keystrokes to the system
    /// </summary>
    /// <param name="keys">The keys to send</param>
    Task PressKey(params string[] keys);

    /// <summary>
    /// Simulate alt key release to prevent undesired behaviors such as Alt+Enter toggling fullscreen mode.
    /// </summary>
    void ReleaseAltModifier();

    /// <summary>
    /// Indicates whether Ctrl is currently pressed.
    /// </summary>
    bool IsCtrlPressed { get; }

    HashSet<string?> UsedKeybinds { get; }
}
