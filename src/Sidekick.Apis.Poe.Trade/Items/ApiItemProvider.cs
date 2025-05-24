using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Items.Models;
using Sidekick.Common;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Items;

public class ApiItemProvider
(
    ICacheProvider cacheProvider,
    ITradeApiClient tradeApiClient,
    ILogger<ApiItemProvider> logger,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService
) : IApiItemProvider
{
    public List<ApiItem> UniqueItems { get; private set; } = [];

    public Dictionary<string, List<ApiItem>> NameAndTypeDictionary { get; } = new();

    public List<(Regex Regex, ApiItem Item)> NameAndTypeRegex { get; } = new();

    /// <inheritdoc/>
    public int Priority => 100;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        if (SidekickConfiguration.IsPoeApiDown) return;

        NameAndTypeDictionary.Clear();
        NameAndTypeRegex.Clear();

        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();
        var cacheKey = $"{game.GetValueAttribute()}_Items";

        var result = await cacheProvider.GetOrSet(cacheKey, () => tradeApiClient.Fetch<ApiCategory>(game, gameLanguageProvider.Language, "data/items"), (cache) => cache.Result.Any());

        var categories = game switch
        {
            GameType.PathOfExile2 => ApiItemConstants.Poe2Categories,
            _ => ApiItemConstants.Poe1Categories,
        };

        foreach (var category in categories)
        {
            FillCategoryItems(game, result.Result, category.Key, category.Value.Category, category.Value.UseRegex);
        }

        UniqueItems = NameAndTypeDictionary.Values.SelectMany(x => x).Where(x => x.IsUnique).OrderByDescending(x => x.Name?.Length).ToList();
    }

    private void FillCategoryItems(GameType game, List<ApiCategory> categories, string categoryId, Category category, bool useRegex = false)
    {
        var categoryItems = categories.SingleOrDefault(x => x.Id == categoryId);
        if (categoryItems == null)
        {
            logger.LogWarning($"[MetadataProvider] The category '{categoryId}' could not be found in the metadata from the API.");
            return;
        }

        for (var i = 0; i < categoryItems.Entries.Count; i++)
        {
            var entry = categoryItems.Entries[i];
            entry.Id = $"{categoryId}.{i}";
            entry.Game = game;
            entry.Category = category;

            if (!string.IsNullOrEmpty(entry.Name))
            {
                FillDictionary(entry.Name, entry);
                if (!entry.IsUnique && useRegex) NameAndTypeRegex.Add((new Regex(Regex.Escape(entry.Name)), entry));
            }

            if (!string.IsNullOrEmpty(entry.Type))
            {
                FillDictionary(entry.Type, entry);
                if (!entry.IsUnique && useRegex) NameAndTypeRegex.Add((new Regex(Regex.Escape(entry.Type)), entry));
            }

            if (!string.IsNullOrEmpty(entry.Text))
            {
                FillDictionary(entry.Text, entry);
                if (!entry.IsUnique && useRegex) NameAndTypeRegex.Add((new Regex(Regex.Escape(entry.Text)), entry));
            }
        }
    }

    private void FillDictionary(string key, ApiItem metadata)
    {
        if (!NameAndTypeDictionary.TryGetValue(key, out var dictionaryEntry))
        {
            dictionaryEntry = new List<ApiItem>();
            NameAndTypeDictionary.Add(key, dictionaryEntry);
        }

        dictionaryEntry.Add(metadata);
    }
}
