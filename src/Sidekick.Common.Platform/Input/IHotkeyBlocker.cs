namespace Sidekick.Common.Platform.Input;

/// <summary>
/// Keeps hotkeys from reaching the game where the global hook can't. On Windows the hook blocks them itself, so this
/// does nothing there; on Linux the hotkeys are grabbed through X11 while the game has focus.
/// </summary>
public interface IHotkeyBlocker
{
    /// <summary>
    /// Tells the blocker which hotkeys to block, and how to know whether a blocked one is used.
    /// </summary>
    /// <param name="keybinds">Gets the hotkeys to block. Read each time the game gains focus.</param>
    /// <param name="isKeybindUsed">Whether a handler uses this hotkey now. When false, the key goes to the game.</param>
    /// <param name="isScrollUsed">Whether Ctrl+scroll is used. Read each time the game gains focus, and on each scroll.</param>
    void SetSource(Func<IEnumerable<string>> keybinds, Func<string, bool> isKeybindUsed, Func<bool> isScrollUsed);

    /// <summary>
    /// Reads the hotkeys again, for example when they were loaded or changed while the game has focus.
    /// </summary>
    void Refresh();

    /// <summary>
    /// Prepares for simulated keys, so they reach the game instead of being blocked by Sidekick itself.
    /// </summary>
    /// <param name="keyStrokes">The keys about to be simulated.</param>
    /// <returns>True when blocking was stopped; call <see cref="Resume"/> after the keys are sent.</returns>
    bool Suspend(IReadOnlyCollection<string> keyStrokes);

    /// <summary>
    /// Blocks the hotkeys again shortly after simulated keys were sent.
    /// </summary>
    void Resume();
}
