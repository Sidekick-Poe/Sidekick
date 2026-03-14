using Microsoft.Extensions.Logging;
using Sidekick.Data.Builder.Ninja;
using Sidekick.Data.Builder.Pseudo;
using Sidekick.Data.Builder.Repoe;
using Sidekick.Data.Builder.Stats;
using Sidekick.Data.Builder.Trade;
using Sidekick.Data.Languages;

namespace Sidekick.Data.Builder;

public class DataBuilder(
    ILogger<DataBuilder> logger,
    NinjaDownloader ninjaDownloader,
    StatBuilder statBuilder,
    PseudoBuilder pseudoBuilder,
    TradeDownloader tradeDownloader,
    TradeBuilder tradeBuilder,
    RepoeDownloader repoeDownloader,
    IGameLanguageProvider gameLanguageProvider)
{
    public async Task DownloadAndBuildAll(
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
        await tradeBuilder.Build(language);
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
