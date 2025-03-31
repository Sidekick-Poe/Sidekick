using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Items.Models;
using Sidekick.Common;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Items;

public class ApiInvariantItemProvider
(
    ICacheProvider cacheProvider,
    IPoeTradeClient poeTradeClient,
    ILogger<ApiInvariantItemProvider> logger,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService
) : IApiInvariantItemProvider
{
    public Dictionary<string, ApiItem> IdDictionary { get; } = new();

    public List<string> UncutGemIds { get; } = [];

    /// <inheritdoc/>
    public int Priority => 100;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        if (SidekickConfiguration.IsPoeApiDown) return;

        IdDictionary.Clear();

        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();
        var cacheKey = $"{game.GetValueAttribute()}_InvariantItems";

        var result = await cacheProvider.GetOrSet(cacheKey, () => poeTradeClient.Fetch<ApiCategory>(game, gameLanguageProvider.InvariantLanguage, "data/items"), (cache) => cache.Result.Any());

        var categories = game switch
        {
            GameType.PathOfExile2 => ApiItemConstants.Poe2Categories,
            _ => ApiItemConstants.Poe1Categories,
        };
        foreach (var category in categories)
        {
            FillCategoryItems(game, result.Result, category.Key, category.Value.Category);
        }

        InitializeUncutGemIds();
    }

    private void FillCategoryItems(GameType game, List<ApiCategory> categories, string categoryId, Category category)
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
            IdDictionary.Add(entry.Id, entry);
        }
    }

    private void InitializeUncutGemIds()
    {
        UncutGemIds.Clear();

        foreach (var item in IdDictionary)
        {
            if (item.Value.Type == "Uncut Skill Gem" || item.Value.Type == "Uncut Spirit Gem" || item.Value.Type == "Uncut Support Gem")
            {
                UncutGemIds.Add(item.Key);
            }
        }
    }
}
