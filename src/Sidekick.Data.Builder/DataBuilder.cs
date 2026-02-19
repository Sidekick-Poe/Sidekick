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
    TradeLeagueBuilder  tradeLeagueBuilder,
    TradeStatBuilder tradeStatBuilder,
    RepoeDownloader repoeDownloader,
    TradeInvariantStatBuilder tradeInvariantStatBuilder,
    IGameLanguageProvider gameLanguageProvider)
{
    private bool InvariantBuilt { get; set; }

    public async Task DownloadAndBuildAll()
    {
        logger.LogInformation("Building all data files.");

        InvariantBuilt = false;
        dataProvider.DeleteAll();


        foreach (var language in gameLanguageProvider.GetList())
        {
            await Download(language);
        }

        await BuildInvariant();

        foreach (var language in gameLanguageProvider.GetList())
        {
            await Build(language);
        }

        logger.LogInformation("Built all data files.");
    }

    public async Task Download(IGameLanguage language)
    {
        logger.LogInformation($"Downloading {language.Code} data files.");

        await tradeDownloader.Download(language);
        await repoeDownloader.Download(language);

        logger.LogInformation($"Downloaded {language.Code} data file.");
    }

    public async Task Build(IGameLanguage language)
    {
        logger.LogInformation($"Building {language.Code} data files.");

        await tradeStatBuilder.Build(language);

        logger.LogInformation($"Built {language.Code} data files.");
    }

    public async Task BuildInvariant()
    {
        if (InvariantBuilt)
        {
            logger.LogInformation("Invariant build already complete.");
            return;
        }

        logger.LogInformation("Building invariant data files.");

        await tradeLeagueBuilder.Build();
        await ninjaDownloader.Download();
        await tradeInvariantStatBuilder.Build();

        InvariantBuilt = true;

        logger.LogInformation("Invariant data files built.");
    }
}
