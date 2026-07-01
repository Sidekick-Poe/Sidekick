using Sidekick.Common.Platform;

namespace Sidekick.Avalonia.Services;

public class AvaloniaApplicationService : IApplicationService
{
    public void Shutdown()
    {
        Environment.Exit(0);
    }

    public bool HasInitialized { get; set; }
}
