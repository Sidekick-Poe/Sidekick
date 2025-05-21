namespace Sidekick.Common.Platform.Linux.Processes;

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

    /// <inheritdoc/>
    public int Priority => 0;
}
