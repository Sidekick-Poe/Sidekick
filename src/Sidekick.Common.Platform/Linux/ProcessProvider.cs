using System.Runtime.Versioning;

namespace Sidekick.Common.Platform.Linux;

[SupportedOSPlatform(PlatformNames.Linux)]
public class ProcessProvider() : IProcessProvider
{
    public string? ClientLogPath
    {
        get
        {
            return null;
        }
    }

    public Task Initialize()
    {
        return Task.CompletedTask;
    }

    public bool IsPathOfExileInFocus
    {
        get
        {
            return true;
        }
    }

    /// <inheritdoc/>
    public bool IsSidekickInFocus
    {
        get
        {
            return true;
        }
    }
}
