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
    public async Task DownloadAndBuild(
        IGameLanguage language,
        bool items = true,
        bool stats = true,
        bool trade = true,
        bool repoe = true,
        bool pseudo = true,
        bool ninja = true)
    {
        logger.LogInformation($"Building {language.Code} data files.");

        if (trade)
        {
            await DownloadAndBuildTrade(language);
        }

        if (repoe)
        {
            await DownloadRepoe(language);
        }

        if (ninja)
        {
            await DownloadNinja(language);
        }

        if (items)
        {
            await BuildItems(language);
        }

        if (pseudo)
        {
            await BuildPseudo(language);
        }

        if (stats)
        {
            await BuildStats(language);
        }

        logger.LogInformation($"Built {language.Code} data files.");
    }

    private async Task DownloadAndBuildTrade(IGameLanguage language)
    {
        logger.LogInformation($"Downloading {language.Code} trade data.");
        await tradeDownloader.Download(language);
        logger.LogInformation($"Building {language.Code} trade data.");
        await leagueBuilder.Build(language);
        await tradeFilterBuilder.Build(language);
        await tradeStatBuilder.Build(language);
        await statsInvariantBuilder.Build(language);
    }

    private async Task BuildItems(IGameLanguage language)
    {
        logger.LogInformation($"Building {language.Code} items data.");
        await itemClassBuilder.Build(language);
        await itemDefinitionBuilder.Build(language);
    }

    private async Task DownloadRepoe(IGameLanguage language)
    {
        logger.LogInformation($"Downloading {language.Code} repoe data.");
        // await repoeDownloader.Download(language);
    }

    private async Task DownloadNinja(IGameLanguage language)
    {
        if (language.Code != gameLanguageProvider.InvariantLanguage.Code) return;

        logger.LogInformation("Downloading ninja data.");
        await ninjaDownloader.Download();
    }

    private async Task BuildStats(IGameLanguage language)
    {
        logger.LogInformation($"Building {language.Code} stats data.");
        await statBuilder.Build(language);
    }

    private async Task BuildPseudo(IGameLanguage language)
    {
        logger.LogInformation($"Building {language.Code} pseudo data.");
        await pseudoBuilder.Build(language);
    }
}
