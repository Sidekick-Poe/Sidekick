using System.Runtime.Versioning;
using Sidekick.Common.Debugging;
using Sidekick.Common.Platform.Linux.X11;

namespace Sidekick.Common.Platform.Linux;

[SupportedOSPlatform(PlatformNames.Linux)]
public class ProcessProvider(X11Session x11Session, DebugService debugService) : IProcessProvider
{
    public string? ClientLogPath
    {
        get
        {
            return null;
        }
    }

    public Task Initialize()
    {
        x11Session.Start();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Without X11, or while a native Wayland window has focus (like the game in Proton's Wayland mode), Sidekick can't
    /// tell which window has focus, so it assumes the game does to keep the hotkeys working.
    /// </summary>
    public bool IsPathOfExileInFocus
    {
        get
        {
            if (debugService.HotkeysInAnyWindow || !x11Session.IsAvailable || !x11Session.IsActiveWindowKnown)
            {
                return true;
            }

            return x11Session.IsPathOfExileActive;
        }
    }

    /// <inheritdoc/>
    public bool IsSidekickInFocus
    {
        get
        {
            if (!x11Session.IsAvailable)
            {
                return true;
            }

            return x11Session.IsSidekickActive;
        }
    }
}
