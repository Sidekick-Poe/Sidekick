using Sidekick.Common.Platform;

namespace Sidekick.Web.Services;

public class WebApplicationService : IApplicationService
{
    public bool SupportsKeybinds => false;

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
