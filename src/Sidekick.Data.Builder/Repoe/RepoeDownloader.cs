using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
namespace Sidekick.Data.Builder.Repoe;

public class RepoeDownloader(
    ILogger<RepoeDownloader> logger,
    DataProvider dataProvider)
{
    private sealed record RepoeLanguageInfo(string Code, string LanguageSlug);

    private sealed record RepoeFile(string FileName, string FilePath);

    private static List<RepoeLanguageInfo> Languages { get; } =
    [
        new("en", ""),
        new("de", "German/"),
        new("es", "Spanish/"),
        new("fr", "French/"),
        new("ja", "Japanese/"),
        new("ko", "Korean/"),
        new("pt", "Portuguese/"),
        new("ru", "Russian/"),
        new("th", "Thai/"),
        new("zh", "Traditional Chinese/"),
    ];

    private static List<RepoeFile> Poe1Files { get; } =
    [
        new("stat_translations", "stat_translations.min.json"),
        new("stats", "stats.min.json"),
    ];

    private static List<RepoeFile> Poe2Files { get; } =
    [
    ];

    private static string GetFileName(IGameLanguage language, string path)
        => $"{path}.{language.Code}.json";

    public async Task Download(IGameLanguage language)
    {
        await DownloadPoe1(language);
        await DownloadPoe2(language);
    }

    private async Task DownloadPoe1(IGameLanguage language)
    {
        using var http = new HttpClient();
        var repoeLanguage = Languages.First(x => x.Code == language.Code);

        foreach (var file in Poe1Files)
        {
            var url = "https://repoe-fork.github.io/" + repoeLanguage.LanguageSlug + file.FilePath;
            await DownloadToFile(http, url, GameType.PathOfExile1, GetFileName(language, file.FileName));
        }
    }

    private async Task DownloadPoe2(IGameLanguage language)
    {
        using var http = new HttpClient();
        var repoeLanguage = Languages.First(x => x.Code == language.Code);

        foreach (var file in Poe2Files)
        {
            var url = "https://repoe-fork.github.io/poe2/" + repoeLanguage.LanguageSlug + file.FilePath;
            await DownloadToFile(http, url, GameType.PathOfExile2, GetFileName(language, file.FileName));
        }
    }

    private async Task DownloadToFile(HttpClient http, string url, GameType game, string fileName)
    {
        try
        {
            logger.LogInformation($"GET {url}");
            using var response = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            await dataProvider.Write(game, $"repoe/{fileName}", await response.Content.ReadAsStreamAsync());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed for {url}: {ex.Message}");
            throw;
        }
    }
}
