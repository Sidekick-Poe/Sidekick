namespace Sidekick.Common.Platform.Input;

/// <summary>
/// Used where the global hook blocks hotkeys itself (Windows), or where nothing can block them.
/// </summary>
public class NoHotkeyBlocker : IHotkeyBlocker
{
    /// <inheritdoc/>
    public void SetSource(Func<IEnumerable<string>> keybinds, Func<string, bool> isKeybindUsed, Func<bool> isScrollUsed)
    {
    }

    /// <inheritdoc/>
    public void Refresh()
    {
    }

    /// <inheritdoc/>
    public bool Suspend(IReadOnlyCollection<string> keyStrokes)
    {
        return false;
    }

    /// <inheritdoc/>
    public void Resume()
    {
    }
}
