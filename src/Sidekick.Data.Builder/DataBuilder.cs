using Microsoft.Extensions.Logging;
using Sidekick.Data.Builder.Items;
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
    ItemBuilder itemBuilder,
    StatsInvariantBuilder statsInvariantBuilder,
    RepoeDownloader repoeDownloader,
    TradeFilterBuilder tradeFilterBuilder,
    IGameLanguageProvider gameLanguageProvider)
{
    public async Task DownloadAndBuildAll(
        bool items = true,
        bool stats = true,
        bool trade = true,
        bool repoe = true,
        bool pseudo = true,
        bool ninja = true)
    {
        logger.LogInformation("Building all data files.");

        foreach (var language in gameLanguageProvider.GetList())
        {
            if (trade)
            {
                await DownloadAndBuildTrade(language);
            }

            if (repoe)
            {
                await DownloadRepoe(language);
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

        }

        if (ninja)
        {
            await DownloadNinja();
        }

        logger.LogInformation("Built all data files.");
    }

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

        if (ninja)
        {
            await DownloadNinja();
        }

        logger.LogInformation($"Built {language.Code} data files.");
    }

    private async Task DownloadAndBuildTrade(IGameLanguage language)
    {
        logger.LogInformation($"Downloading {language.Code} trade data.");
        await tradeDownloader.Download(language);
        logger.LogInformation($"Building {language.Code} trade data.");
        await statsInvariantBuilder.Build(language);
        await leagueBuilder.Build(language);
        await tradeFilterBuilder.Build(language);
    }

    private async Task BuildItems(IGameLanguage language)
    {
        logger.LogInformation($"Building {language.Code} items data.");
        await itemBuilder.Build(language);
    }

    private async Task DownloadRepoe(IGameLanguage language)
    {
        logger.LogInformation($"Downloading {language.Code} repoe data.");
        await repoeDownloader.Download(language);
    }

    private async Task DownloadNinja()
    {
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
