using Microsoft.Extensions.Logging;
using Sidekick.Data.Builder.Ninja;
using Sidekick.Data.Builder.Repoe;
using Sidekick.Data.Builder.Stats;
using Sidekick.Data.Builder.Trade;
using Sidekick.Data.Languages;

namespace Sidekick.Data.Builder;

public class DataBuilder(
    ILogger<DataBuilder> logger,
    DataProvider dataProvider,
    StatBuilder statBuilder,
    NinjaDownloader ninjaDownloader,
    TradeDownloader tradeDownloader,
    TradeLeagueBuilder  tradeLeagueBuilder,
    TradeStatBuilder tradeStatBuilder,
    RepoeDownloader repoeDownloader,
    TradeInvariantStatBuilder tradeInvariantStatBuilder,
    IGameLanguageProvider gameLanguageProvider)
{
    public async Task DownloadAndBuildAll()
    {
        logger.LogInformation("Building all data files.");

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
        await statBuilder.Build(language);

        logger.LogInformation($"Built {language.Code} data files.");
    }

    public async Task BuildInvariant()
    {
        logger.LogInformation("Building invariant data files.");

        await tradeLeagueBuilder.Build();
        await tradeInvariantStatBuilder.Build();
        await ninjaDownloader.Download();

        logger.LogInformation("Invariant data files built.");
    }
}
