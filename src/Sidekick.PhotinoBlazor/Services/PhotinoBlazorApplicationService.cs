using Sidekick.Common.Platform;

namespace Sidekick.PhotinoBlazor.Services;

public class PhotinoBlazorApplicationService : IApplicationService
{
    public bool SupportsKeybinds => true;

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
