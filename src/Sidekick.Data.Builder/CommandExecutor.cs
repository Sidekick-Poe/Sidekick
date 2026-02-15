using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Data.Game;
using Sidekick.Data.Ninja;
using Sidekick.Data.Options;
using Sidekick.Data.Trade;

namespace Sidekick.Data;

internal sealed class CommandExecutor(
    ILogger<CommandExecutor> logger,
    NinjaDownloader ninjaDownloader,
    TradeDownloader tradeDownloader,
    RepoeDownloader repoeDownloader,
    IOptions<DataOptions> options)
{
    public async Task<int> Execute(string[] args)
    {
        try
        {
            if (args.Contains("--help") || args.Contains("-h"))
            {
                PrintHelp();
                return 1;
            }

            if (!string.IsNullOrEmpty(options.Value.DataFolder))
            {
                Directory.Delete(options.Value.DataFolder, true);
                Directory.CreateDirectory(options.Value.DataFolder);
            }

            await ninjaDownloader.DownloadAll();
            await tradeDownloader.DownloadAll();
            await repoeDownloader.DownloadAll();

            Console.WriteLine("Done.");
            return 0;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Failed to execute command.");
            return 2;
        }
    }

    private void PrintHelp()
    {
        logger.LogInformation(@"Sidekick.Data - Console downloader (decoupled)

Usage:
  download-trade --folder <PATH> [--languages en,de,es] [--paths items,stats,static,filters]
  analyze --folder <PATH>
  download-ninja --folder <PATH> [--poe1 <LEAGUE>] [--poe2 <LEAGUE>]

Notes:
  - Trade data downloads for both games (poe1, poe2). Also downloads 'leagues' in English.
  - Ninja data requires league names; pass one or both of --poe1/--poe2.
");
    }
}