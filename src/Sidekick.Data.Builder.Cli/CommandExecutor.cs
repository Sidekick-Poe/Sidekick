using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Data.Builder.Game;
using Sidekick.Data.Builder.Ninja;
using Sidekick.Data.Builder.Trade;

namespace Sidekick.Data.Builder.Cli;

internal sealed class CommandExecutor(
    ILogger<CommandExecutor> logger,
    NinjaDownloader ninjaDownloader,
    TradeDownloader tradeDownloader,
    TradeStatBuilder tradeStatBuilder,
    RepoeDownloader repoeDownloader,
    DataProvider dataProvider,
    IGameLanguageProvider gameLanguageProvider)
{
    public async Task<int> Execute()
    {
        try
        {
            dataProvider.DeleteAll();

            foreach (var language in gameLanguageProvider.GetList())
            {
                var languageDetails = gameLanguageProvider.GetLanguage(language.LanguageCode);

                await tradeDownloader.Download(languageDetails);
                await repoeDownloader.Download(languageDetails);
            }

            await ninjaDownloader.Download();

            foreach (var language in gameLanguageProvider.GetList())
            {
                var languageDetails = gameLanguageProvider.GetLanguage(language.LanguageCode);

                await tradeStatBuilder.Build(languageDetails);
            }

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
