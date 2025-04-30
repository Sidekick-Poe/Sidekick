using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Sidekick.Common;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Linux;

namespace Sidekick.PhotinoBlazor.Services;

public class PhotinoBlazorApplicationService(ILogger<PhotinoBlazorApplicationService> logger) : IApplicationService
{
    public bool SupportsKeybinds => true;

    public int Priority => 9000;

    public Task Initialize()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            if (!PackageVerifier.IsXselInstalled())
            {
                logger.LogError("[ApplicationService] xsel is not installed. Please install xsel to enable clipboard functionality.");
                SidekickConfiguration.IsXselPackageMissing = true;
            }
        }

        return Task.CompletedTask;
    }

    public void Shutdown()
    {
        Environment.Exit(0);
    }
}
