using Sidekick.Common.Platform;

namespace Sidekick.Linux.Platform;

public class X11ProcessProvider : IProcessProvider
{
    public string? ClientLogPath => null;

    public bool IsPathOfExileInFocus => IsActiveWindowMatch("Path of Exile", "Path of Exile 2");

    public bool IsSidekickInFocus => IsActiveWindowMatch("Sidekick");

    public int Priority => 0;

    public Task Initialize()
    {
        // TODO: Wire X11 _NET_ACTIVE_WINDOW / _NET_WM_NAME polling or event hooks.
        return Task.CompletedTask;
    }

    private static bool IsActiveWindowMatch(params string[] titles)
    {
        // TODO: Query X11 for the active window title and compare.
        return false;
    }
}
