using Sidekick.Common.Initialization;

namespace Sidekick.Common.Platform.Input;

/// <summary>
/// Keeps hotkeys from reaching the game where the global hook can't. On Windows the hook blocks them itself, so its
/// blocker does nothing; on Linux the hotkeys are grabbed through X11 while the game has focus.
/// </summary>
public interface IHotkeyBlocker : IInitializableService
{
    /// <summary>
    /// Prepares for simulated keys, so they reach the game instead of being blocked by Sidekick itself. Called by the
    /// input provider, the only one that knows when keys are about to be simulated.
    /// </summary>
    /// <param name="keyStrokes">The keys about to be simulated.</param>
    /// <returns>True when blocking was stopped; call <see cref="Resume"/> after the keys are sent.</returns>
    bool Suspend(IReadOnlyCollection<string> keyStrokes);

    /// <summary>
    /// Blocks the hotkeys again shortly after simulated keys were sent.
    /// </summary>
    void Resume();
}
