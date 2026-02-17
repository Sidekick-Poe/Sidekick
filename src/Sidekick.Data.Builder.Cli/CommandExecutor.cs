using Microsoft.Extensions.Logging;
using Sidekick.Data.Builder.Game;
using Sidekick.Data.Builder.Ninja;
using Sidekick.Data.Builder.Trade;

namespace Sidekick.Data.Builder.Cli;

internal sealed class CommandExecutor(
    ILogger<CommandExecutor> logger,
    NinjaDownloader ninjaDownloader,
    TradeDownloader tradeDownloader,
    RepoeDownloader repoeDownloader,
    DataProvider dataProvider)
{
    public async Task<int> Execute()
    {
        try
        {
            dataProvider.DeleteAll();
            await tradeDownloader.DownloadAll();
            await repoeDownloader.DownloadAll();
            await ninjaDownloader.DownloadAll();

            logger.LogInformation("Done.");
            return 0;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Failed to execute command.");
            return 2;
        }
    }
}