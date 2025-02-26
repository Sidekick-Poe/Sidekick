using Sidekick.Common.Initialization;

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

    HashSet<string?> UsedKeybinds { get; }
}
