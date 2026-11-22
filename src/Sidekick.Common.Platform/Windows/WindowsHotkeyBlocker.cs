using System.Runtime.Versioning;
using Sidekick.Common.Platform.Input;

namespace Sidekick.Common.Platform.Windows;

/// <summary>
/// Does nothing: on Windows, the global hook keeps the hotkeys from reaching the game itself.
/// </summary>
[SupportedOSPlatform(PlatformNames.Windows)]
public class WindowsHotkeyBlocker : IHotkeyBlocker
{
    /// <inheritdoc/>
    public Task Initialize() => Task.CompletedTask;

    /// <inheritdoc/>
    public bool Suspend(IReadOnlyCollection<string> keyStrokes) => false;

    /// <inheritdoc/>
    public void Resume()
    {
    }
}
