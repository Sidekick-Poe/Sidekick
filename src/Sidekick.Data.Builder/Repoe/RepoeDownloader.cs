using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Common.Enums;
using Sidekick.Data.Builder.Repoe.Models.Items;
using Sidekick.Data.Builder.Repoe.Models.Stats;
using Sidekick.Data.Languages;
namespace Sidekick.Data.Builder.Repoe;

public class RepoeDownloader(
    ILogger<RepoeDownloader> logger,
    IOptions<SidekickConfiguration> configuration,
    RawDataProvider rawDataProvider)
{
    private sealed record RepoeLanguageInfo(string Code, string LanguageSlug);

    private sealed record RepoeFile(RepoeFileType Type, string FileName, string FilePath);

    private enum RepoeFileType
    {
        StatTranslations,
        BaseItems,
        ItemClasses,
        Uniques,
    }

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
        new(RepoeFileType.StatTranslations, "stat_translations", "stat_translations.json"),
        new(RepoeFileType.BaseItems, "base_items", "base_items.json"),
        new(RepoeFileType.ItemClasses, "item_classes", "item_classes.json"),
        new(RepoeFileType.Uniques, "uniques", "uniques.json"),
    ];

    private static List<RepoeFile> Poe2Files { get; } =
    [
        new(RepoeFileType.StatTranslations, "stat_translations.advanced", "stat_translations/advanced_mod_stat_descriptions.json"),
        new(RepoeFileType.StatTranslations, "stat_translations.endgamemap", "stat_translations/endgame_map_stat_descriptions.json"),
        new(RepoeFileType.StatTranslations, "stat_translations.heist", "stat_translations/heist_equipment_stat_descriptions.json"),
        new(RepoeFileType.StatTranslations, "stat_translations.leaguestone", "stat_translations/leaguestone_stat_descriptions.json"),
        new(RepoeFileType.StatTranslations, "stat_translations.map", "stat_translations/map_stat_descriptions.json"),
        new(RepoeFileType.StatTranslations, "stat_translations.sanctum", "stat_translations/sanctum_relic_stat_descriptions.json"),
        new(RepoeFileType.StatTranslations, "stat_translations.sentinel", "stat_translations/sentinel_stat_descriptions.json"),
        new(RepoeFileType.StatTranslations, "stat_translations.descriptions", "stat_translations/stat_descriptions.json"),
        new(RepoeFileType.StatTranslations, "stat_translations.tablet", "stat_translations/tablet_stat_descriptions.json"),
        new(RepoeFileType.BaseItems, "base_items", "base_items.json"),
        new(RepoeFileType.ItemClasses, "item_classes", "item_classes.json"),
        new(RepoeFileType.Uniques, "uniques", "uniques.json"),
    ];

    private static string GetFileName(IGameLanguage language, string path)
        => $"{path}.{language.Code}.json";

    public async Task<Dictionary<string, RepoeItemClass>> ReadItemClasses(GameType game, string language)
    {
        var files = game == GameType.PathOfExile1 ? Poe1Files : Poe2Files;
        var result = new Dictionary<string, RepoeItemClass>();
        foreach (var file in files.Where(x => x.Type == RepoeFileType.ItemClasses))
        {
            var data = await rawDataProvider.Read<Dictionary<string, RepoeItemClass>>($"{game.GetValueAttribute()}/raw/repoe/{file.FileName}.{language}.json");
            foreach (var entry in data)
            {
                result.Add(entry.Key, entry.Value);
            }
        }

        return result;
    }

    public async Task<Dictionary<string, RepoeBaseItem>> ReadBaseItems(GameType game, string language)
    {
        var files = game == GameType.PathOfExile1 ? Poe1Files : Poe2Files;
        var result = new Dictionary<string, RepoeBaseItem>();
        foreach (var file in files.Where(x => x.Type == RepoeFileType.BaseItems))
        {
            var data = await rawDataProvider.Read<Dictionary<string, RepoeBaseItem>>($"{game.GetValueAttribute()}/raw/repoe/{file.FileName}.{language}.json");
            foreach (var entry in data)
            {
                result.Add(entry.Key, entry.Value);
            }
        }

        return result;
    }

    public async Task<Dictionary<string, RepoeUniqueItem>> ReadUniques(GameType game, string language)
    {
        var files = game == GameType.PathOfExile1 ? Poe1Files : Poe2Files;
        var result = new Dictionary<string, RepoeUniqueItem>();
        foreach (var file in files.Where(x => x.Type == RepoeFileType.Uniques))
        {
            var data = await rawDataProvider.Read<Dictionary<string, RepoeUniqueItem>>($"{game.GetValueAttribute()}/raw/repoe/{file.FileName}.{language}.json");
            foreach (var entry in data)
            {
                result.Add(entry.Key, entry.Value);
            }
        }

        return result;
    }

    public async Task<List<RepoeStatTranslation>> ReadStatTranslations(GameType game, string language)
    {
        var files = game == GameType.PathOfExile1 ? Poe1Files : Poe2Files;
        var result = new List<RepoeStatTranslation>();
        foreach (var file in files.Where(x => x.Type == RepoeFileType.StatTranslations))
        {
            result.AddRange(await rawDataProvider.Read<List<RepoeStatTranslation>>($"{game.GetValueAttribute()}/raw/repoe/{file.FileName}.{language}.json"));
        }

        return result;
    }

    public async Task Download(IGameLanguage language)
    {
        try
        {
            await DownloadPoe1(language);
            await DownloadPoe2(language);
        }
        catch (Exception ex)
        {
            if (configuration.Value.ApplicationType == SidekickApplicationType.DataBuilder || configuration.Value.ApplicationType == SidekickApplicationType.Test)
            {
                throw;
            }

            logger.LogError(ex, $"Failed to download repoe data for {language.Code}.");
        }
    }

    private async Task DownloadPoe1(IGameLanguage language)
    {
        using var http = new HttpClient();
        var repoeLanguage = Languages.First(x => x.Code == language.Code);

        foreach (var file in Poe1Files)
        {
            var url = "https://repoe-fork.github.io/" + repoeLanguage.LanguageSlug + file.FilePath;
            try
            {
                logger.LogInformation($"GET {url}");

                using var response = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                await rawDataProvider.Write($"{GameType.PathOfExile1.GetValueAttribute()}/raw/repoe/{GetFileName(language, file.FileName)}", await response.Content.ReadAsStreamAsync());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed for {url}: {ex.Message}");
                throw;
            }
        }
    }

    private async Task DownloadPoe2(IGameLanguage language)
    {
        using var http = new HttpClient();
        var repoeLanguage = Languages.First(x => x.Code == language.Code);

        foreach (var file in Poe2Files)
        {
            var url = "https://repoe-fork.github.io/poe2/" + repoeLanguage.LanguageSlug + file.FilePath;
            try
            {
                logger.LogInformation($"GET {url}");

                using var response = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                await rawDataProvider.Write($"{GameType.PathOfExile2.GetValueAttribute()}/raw/repoe/{GetFileName(language, file.FileName)}", await response.Content.ReadAsStreamAsync());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed for {url}: {ex.Message}");
                throw;
            }
        }
    }
}
