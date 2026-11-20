using Sidekick.Common.Initialization;
using Sidekick.Common.Platform.EventArgs;

namespace Sidekick.Common.Platform;

/// <summary>
/// Service providing keyboard functions
/// </summary>
public interface IInputProvider : IInitializableService
{
    /// <summary>
    /// Event that indicates that a key was pressed
    /// </summary>
    event Action<string> OnKeyDown;

    /// <summary>
    /// Event triggered when a mouse button is pressed anywhere on the screen, with the position in screen pixels.
    /// Raised on the hook thread. On Linux, only presses in X11 windows are seen.
    /// </summary>
    event Action<int, int> OnMouseDown;

    /// <summary>
    /// Event triggered when the user scrolls down.
    /// </summary>
    event Action<ScrollEventArgs> OnScrollDown;

    /// <summary>
    /// Event triggered when the user scrolls up.
    /// </summary>
    event Action<ScrollEventArgs> OnScrollUp;

    /// <summary>
    /// Register keyboard hooks to capture keybinds.
    /// </summary>
    void RegisterHooks();

    /// <summary>
    /// Unregister keyboard hooks to capture keybinds.
    /// </summary>
    void UnregisterHooks();

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
    /// Enables the keyboard provider to begin processing keyboard-related events and interactions.
    /// </summary>
    void Enable();

    /// <summary>
    /// Stops processing keyboard-related events.
    /// </summary>
    void Disable();

    Task WiggleMouse();

    HashSet<string?> UsedKeybinds { get; }

    /// <summary>
    /// While a hotkey is being changed in the settings, key presses only raise <see cref="OnKeyDown"/>: they don't run any hotkey and, where the platform allows it, don't reach any window.
    /// </summary>
    bool IsRecordingKeybind { get; set; }

    /// <summary>
    /// Raised when <see cref="IsRecordingKeybind"/> is turned off, for example when the window loses focus, so that the hotkey editor stops listening.
    /// </summary>
    event Action? OnRecordingKeybindStopped;

    short MousePositionX { get; }
    short MousePositionY { get; }
}
