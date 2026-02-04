using Sidekick.Common.Platform;
namespace Sidekick.Electron.Services;

public class WebApplicationService : IApplicationService
{
    public bool SupportsKeybinds => false;

    public bool SupportsAuthentication => false;

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
