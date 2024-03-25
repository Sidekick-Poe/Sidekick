using Sidekick.Common.Initialization;

namespace Sidekick.Common.Platform;

/// <summary>
///     Service providing keyboard functions
/// </summary>
public interface IKeyboardProvider : IInitializableService
{
    /// <summary>
    ///     Event that indicates that a key was pressed
    /// </summary>
    event Action<string> OnKeyDown;

    /// <summary>
    ///     Command to send keystrokes to the system
    /// </summary>
    /// <param name="keys">The keys to send</param>
    Task PressKey(params string[] keys);
}
