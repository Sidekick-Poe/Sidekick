using Sidekick.Common.Platform;
namespace Sidekick.Electron.Services;

public class ElectronApplicationService : IApplicationService
{
    public bool SupportsKeybinds => true;

    public bool SupportsAuthentication => true;

    public bool SupportsHardwareAcceleration => true;

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
