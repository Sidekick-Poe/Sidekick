using Sidekick.Common.Platform;

namespace Sidekick.Linux.Platform;

public sealed class LinuxApplicationService : IApplicationService
{
    public bool SupportsKeybinds => true;

    public bool SupportsAuthentication => true;

    public bool SupportsHardwareAcceleration => false;

    public int Priority => 9000;

    public Task Initialize()
    {
        return Task.CompletedTask;
    }

    public void Shutdown()
    {
        Environment.Exit(0);
    }
}
