using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Data.Builder.Game;
using Sidekick.Data.Builder.Ninja;
using Sidekick.Data.Builder.Trade;

namespace Sidekick.Data.Builder;

public class DataBuilder(
    ILogger<DataBuilder> logger,
    DataProvider dataProvider,
    NinjaDownloader ninjaDownloader,
    TradeDownloader tradeDownloader,
    TradeStatBuilder tradeStatBuilder,
    RepoeDownloader repoeDownloader,
    TradeInvariantStatBuilder tradeInvariantStatBuilder,
    IGameLanguageProvider gameLanguageProvider)
{
    private bool InvariantBuilt { get; set; }

    public async Task BuildAll()
    {
        logger.LogInformation("Building all data files.");

        InvariantBuilt = false;
        dataProvider.DeleteAll();

        await BuildInvariant();

        foreach (var language in gameLanguageProvider.GetList())
        {
            var languageDetails = gameLanguageProvider.GetLanguage(language.LanguageCode);
            await Build(languageDetails);
        }

        logger.LogInformation("Built all data files.");
    }

    public async Task Build(IGameLanguage language)
    {
        logger.LogInformation($"Building {language.Code} data file.");

        await tradeDownloader.Download(language);
        await repoeDownloader.Download(language);

        await BuildInvariant();

        await tradeStatBuilder.Build(language);

        logger.LogInformation($"Built {language.Code} data file.");
    }

    public async Task BuildInvariant()
    {
        if (InvariantBuilt)
        {
            logger.LogInformation("Invariant build already complete.");
            return;
        }

        logger.LogInformation("Building invariant data file.");

        await ninjaDownloader.Download();

        await tradeInvariantStatBuilder.Build();

        InvariantBuilt = true;

        logger.LogInformation("Invariant data file built.");
    }
}
