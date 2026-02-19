using Microsoft.Extensions.Logging;

namespace Sidekick.Data.Builder.Cli;

internal sealed class CommandExecutor(
    ILogger<CommandExecutor> logger,
    DataBuilder dataBuilder)
{
    public async Task<int> Execute()
    {
        try
        {
            await dataBuilder.DownloadAndBuildAll();
            return 0;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Failed to execute command.");
            return 2;
        }
    }
}
