using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Items;

namespace Sidekick.Data.Builder.Game;

public class RepoeDownloader(
    ILogger<RepoeDownloader> logger,
    DataProvider dataProvider)
{
    private sealed record RepoeLanguageInfo(string Code, string LanguageSlug);

    private sealed record RepoeFile(string FileName, string FilePath);

    private static readonly List<RepoeLanguageInfo> Languages = new()
    {
        new("en", ""),
        new("de", "German/"),
        new("es", "Spanish/"),
        new("fr", "French/"),
        new("ja", "Japanese/"),
        new("ko", "Korean/"),
        new("en", "Portuguese/"),
        new("ru", "Russian/"),
        new("th", "Thai/"),
        new("zh", "Traditional Chinese/"),
    };

    private static string GetFileName(string langCode, string path)
        => $"{path}.{langCode}.json";

    public async Task DownloadAll()
    {
        await DownloadPoe1();
        await DownloadPoe2();
    }

    private async Task DownloadPoe1()
    {
        using var http = new HttpClient();

        List<RepoeFile> files =
        [
            new RepoeFile("stats", "stats.min.json"),
        ];

        foreach (var file in files)
        {
            foreach (var language in Languages)
            {
                var url = "https://repoe-fork.github.io/" + language.LanguageSlug + file.FilePath;
                await DownloadToFile(http, url, GameType.PathOfExile1, GetFileName(language.Code, file.FileName));
            }
        }
    }

    private async Task DownloadPoe2()
    {
        using var http = new HttpClient();

        List<RepoeFile> files =
        [
        ];

        foreach (var file in files)
        {
            foreach (var language in Languages)
            {
                var url = "https://repoe-fork.github.io/poe2/" + language.LanguageSlug + file.FilePath;
                await DownloadToFile(http, url, GameType.PathOfExile2, GetFileName(language.Code, file.FileName));
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
            await dataProvider.Write(game, $"game/{fileName}", await response.Content.ReadAsStreamAsync());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed for {url}: {ex.Message}");
        }
    }
}