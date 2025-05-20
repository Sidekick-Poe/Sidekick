using Microsoft.Extensions.Logging;

namespace Sidekick.Common.Platform.Linux.Processes;

public class ProcessProvider(ILogger<ProcessProvider> logger) : IProcessProvider
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

    /// <inheritdoc/>
    public int Priority => 0;
}
