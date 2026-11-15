namespace Sidekick.Common.Debugging;

/// <summary>
/// Test options, turned on from the Development page, that make some features easier to test.
/// They are off at every launch, so they never affect users or normal debugging.
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
}
