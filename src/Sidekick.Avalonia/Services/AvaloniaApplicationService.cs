using Sidekick.Common.Platform;

namespace Sidekick.Avalonia.Services;

public class AvaloniaApplicationService : IApplicationService
{
    public int Priority => 9000;

    public Task Initialize()
    {
        return Task.CompletedTask;
    }

    public void Shutdown()
    {
        Environment.Exit(0);
    }

    public bool HasInitialized { get; set; }
}
