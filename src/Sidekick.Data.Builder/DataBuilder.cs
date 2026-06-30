using Microsoft.Extensions.Logging;
using Sidekick.Data.Builder.ItemClasses;
using Sidekick.Data.Builder.ItemDefinitions;
using Sidekick.Data.Builder.Leagues;
using Sidekick.Data.Builder.Ninja;
using Sidekick.Data.Builder.Pseudo;
using Sidekick.Data.Builder.Repoe;
using Sidekick.Data.Builder.Stats;
using Sidekick.Data.Builder.StatsInvariant;
using Sidekick.Data.Builder.Trade;
using Sidekick.Data.Languages;

namespace Sidekick.Data.Builder;

public class DataBuilder(
    ILogger<DataBuilder> logger,
    LeagueBuilder leagueBuilder,
    NinjaDownloader ninjaDownloader,
    StatBuilder statBuilder,
    PseudoBuilder pseudoBuilder,
    TradeDownloader tradeDownloader,
    ItemDefinitionBuilder itemDefinitionBuilder,
    ItemClassBuilder itemClassBuilder,
    StatsInvariantBuilder statsInvariantBuilder,
    RepoeDownloader repoeDownloader,
    TradeFilterBuilder tradeFilterBuilder,
    TradeStatBuilder tradeStatBuilder,
    IGameLanguageProvider gameLanguageProvider)
{
    public async Task DownloadRawFiles(
        IGameLanguage language,
        bool trade = true,
        bool repoe = true,
        bool ninja = true)
    {
        logger.LogInformation($"Downloading {language.Code} data files.");

        if (trade)
        {
            logger.LogInformation($"Downloading {language.Code} trade data.");
            await tradeDownloader.Download(language);
            logger.LogInformation($"Downloaded {language.Code} trade data.");
        }

        if (repoe)
        {
            logger.LogInformation($"Downloading {language.Code} repoe data.");
            await repoeDownloader.Download(language);
            logger.LogInformation($"Downloaded {language.Code} repoe data.");
        }

        if (ninja)
        {
            if (language.Code != gameLanguageProvider.InvariantLanguage.Code) return;

            logger.LogInformation("Downloading ninja data.");
            await ninjaDownloader.Download();
            logger.LogInformation($"Downloaded {language.Code} ninja data.");
        }

        logger.LogInformation($"Downloaded {language.Code} data files.");
    }

    public async Task BuildDataFiles(
        IGameLanguage language,
        bool items = true,
        bool stats = true,
        bool trade = true,
        bool pseudo = true)
    {
        logger.LogInformation($"Building {language.Code} data files.");

        if (trade)
        {
            logger.LogInformation($"Building {language.Code} trade data.");
            await leagueBuilder.Build(language);
            await tradeFilterBuilder.Build(language);
            await tradeStatBuilder.Build(language);
            await statsInvariantBuilder.Build(language);
            logger.LogInformation($"Built {language.Code} trade data.");
        }

        if (items)
        {
            logger.LogInformation($"Building {language.Code} items data.");
            await itemClassBuilder.Build(language);
            await itemDefinitionBuilder.Build(language);
            logger.LogInformation($"Built {language.Code} items data.");
        }

        if (pseudo)
        {
            logger.LogInformation($"Building {language.Code} pseudo data.");
            await pseudoBuilder.Build(language);
            logger.LogInformation($"Built {language.Code} pseudo data.");
        }

        if (stats)
        {
            logger.LogInformation($"Building {language.Code} stats data.");
            await statBuilder.Build(language);
            logger.LogInformation($"Built {language.Code} stats data.");
        }

        logger.LogInformation($"Built {language.Code} data files.");
    }
}
