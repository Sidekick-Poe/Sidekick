using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Data.Builder.Trade;

public class TradeDownloader(
    ILogger<TradeDownloader> logger,
    DataProvider dataProvider)
{
    private static string GetApiBase(IGameLanguage language, GameType game)
    {
        return game == GameType.PathOfExile2 ? language.Poe2TradeApiBaseUrl : language.PoeTradeApiBaseUrl;
    }

    public async Task Download(IGameLanguage language)
    {
        await DownloadPath(DataType.TradeRawItems, language, "items");
        await DownloadPath(DataType.TradeRawStats, language, "stats");
        await DownloadPath(DataType.TradeRawStatic, language, "static");
        await DownloadPath(DataType.TradeRawFilters, language, "filters");
    }

    public async Task DownloadPath(DataType dataType, IGameLanguage language, string path)
    {
        using var http = new HttpClient();
        http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Sidekick.Data", "1.0"));
        http.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");

        foreach (var game in new[]
                 {
                     GameType.PathOfExile1,
                     GameType.PathOfExile2
                 })
        {
            var url = GetApiBase(language, game) + "data/" + path;
            await DownloadToFile(http, url, game, dataType, language);
        }
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
