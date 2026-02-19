using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;

namespace Sidekick.Data.Builder.Trade;

public class TradeDownloader(
    ILogger<TradeDownloader> logger,
    DataProvider dataProvider)
{
    private static string GetFileName(IGameLanguage language, string path)
        => $"{path}.{language.Code}.json";

    private static string GetApiBase(IGameLanguage language, GameType game)
    {
        return game == GameType.PathOfExile2 ? language.Poe2TradeApiBaseUrl : language.PoeTradeApiBaseUrl;
    }

    public async Task Download(IGameLanguage language)
    {
        using var http = new HttpClient();
        http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Sidekick.Data", "1.0"));
        http.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");

        List<string> paths = ["items", "stats", "static", "filters"];

        foreach (var game in new[]
                 {
                     GameType.PathOfExile1,
                     GameType.PathOfExile2
                 })
        {
            // Download leagues once (English, invariant)
            var leaguesUrl = GetApiBase(language, game) + "data/leagues";
            await DownloadToFile(http, leaguesUrl, game, GetFileName(language, "leagues"));

            foreach (var path in paths)
            {
                var url = GetApiBase(language, game) + "data/" + path;
                await DownloadToFile(http, url, game, GetFileName(language, path));
            }
        }
    }

    private async Task DownloadToFile(HttpClient http, string url, GameType game, string fileName)
    {
        try
        {
            logger.LogInformation($"GET {url}");
            using var response = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            await dataProvider.Write(game, $"trade/raw/{fileName}", await response.Content.ReadAsStreamAsync());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed for {url}: {ex.Message}");
            throw;
        }
    }
}
