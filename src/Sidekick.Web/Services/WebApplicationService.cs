using Sidekick.Common.Platform;

namespace Sidekick.Web.Services;

public class WebApplicationService : IApplicationService
{
    public void Shutdown()
    {
        Environment.Exit(0);
    }

    /// <inheritdoc />
    public bool HasInitialized { get; set; }
}
