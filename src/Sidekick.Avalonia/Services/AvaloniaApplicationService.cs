using Sidekick.Common.Platform;

namespace Sidekick.Avalonia.Services;

public class AvaloniaApplicationService : IApplicationService
{
    public int Priority => 0;

    public Task Initialize()
    {
        return Task.CompletedTask;
    }

    public void Shutdown() {}

    public bool HasInitialized { get; set; }
}
