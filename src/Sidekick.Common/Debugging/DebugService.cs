using System.Diagnostics;

namespace Sidekick.Common.Debugging;

/// <summary>
/// Options that make Sidekick easier to develop and test.
/// The Development page options are off at every launch, so they never affect users or normal debugging.
/// </summary>
public class DebugService
{
    /// <summary>
    /// When on, the setup page always shows its form, and the update page shows sample release notes.
    /// </summary>
    public bool IsDebug { get; set; }

    /// <summary>
    /// When on, hotkeys work in any window, to test them without Path of Exile.
    /// Kept apart from <see cref="IsDebug"/>, since chat commands like F5 would then also fire in the IDE.
    /// </summary>
    public bool HotkeysInAnyWindow { get; set; }

    /// <summary>
    /// Developer mode shows the DevTools, skips the update check, and reads the game data from the repository.
    /// Setting the SIDEKICK_DEVELOPER_MODE environment variable to 1 forces it, to test a build without a debugger.
    /// </summary>
    public bool IsDeveloperMode { get; } = Debugger.IsAttached || Environment.GetEnvironmentVariable("SIDEKICK_DEVELOPER_MODE") == "1";
}
