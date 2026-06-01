using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Data.Languages;

namespace Sidekick.Data.Builder.Trade;

public class TradeDownloader(
    ILogger<TradeDownloader> logger,
    IOptions<SidekickConfiguration> configuration,
    DataProvider dataProvider)
{
    private static string GetApiBase(IGameLanguage language, GameType game)
    {
        return game == GameType.PathOfExile2 ? language.Poe2TradeApiBaseUrl : language.PoeTradeApiBaseUrl;
    }

    public async Task Download(IGameLanguage language)
    {
        try
        {
            foreach (var game in new[]
                     {
                         GameType.PathOfExile1,
                         GameType.PathOfExile2
                     })
            {
                await DownloadPath(DataType.RawTradeItems, game, language, "items");
                await DownloadPath(DataType.RawTradeStats, game, language, "stats");
                await DownloadPath(DataType.RawTradeStatic, game, language, "static");
                await DownloadPath(DataType.RawTradeFilters, game, language, "filters");
            }
        }
        catch (Exception ex)
        {
            if (configuration.Value.ApplicationType == SidekickApplicationType.DataBuilder || configuration.Value.ApplicationType == SidekickApplicationType.Test)
            {
                throw;
            }

            logger.LogError(ex, $"Failed to download trade data for {language.Code}.");
        }
    }

    public async Task DownloadPath(DataType dataType, GameType game, IGameLanguage language, string path)
    {
        using var http = new HttpClient();
        http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Sidekick.Data", "1.0"));
        http.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");

        var url = GetApiBase(language, game) + "data/" + path;
        await DownloadToFile(http, url, game, dataType, language);
    }

    private async Task DownloadToFile(HttpClient http, string url, GameType game, DataType dataType, IGameLanguage language)
    {
        try
        {
            logger.LogInformation($"GET {url}");
            using var response = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            await dataProvider.Write(game, dataType, language, await response.Content.ReadAsStreamAsync());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed for {url}: {ex.Message}");
            throw;
        }
    }
}
