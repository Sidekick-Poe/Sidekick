using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Items.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Items;

public class ApiInvariantItemProvider
(
    ICacheProvider cacheProvider,
    ITradeApiClient tradeApiClient,
    ILogger<ApiInvariantItemProvider> logger,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService
) : IApiInvariantItemProvider
{
    public Dictionary<string, ApiItem> IdDictionary { get; } = new();

    public Dictionary<string, ApiItem> NameDictionary { get; } = new();

    public string UncutSkillGemId { get; private set; } = string.Empty;
    public string UncutSupportGemId { get; private set; } = string.Empty;
    public string UncutSpiritGemId { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public int Priority => 100;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        IdDictionary.Clear();

        var game = await settingsService.GetGame();
        var cacheKey = $"{game.GetValueAttribute()}_InvariantItems";

        var result = await cacheProvider.GetOrSet(cacheKey, () => tradeApiClient.FetchData<ApiCategory>(game, gameLanguageProvider.InvariantLanguage, "items"), (cache) => cache.Result.Any());
        if (result == null) throw new SidekickException("Could not fetch items from the trade API.");

        var categories = game switch
        {
            GameType.PathOfExile2 => ApiItemConstants.Poe2Categories,
            _ => ApiItemConstants.Poe1Categories,
        };
        foreach (var category in categories)
        {
            FillCategoryItems(result.Result, category.Key, category.Value.Category);
        }

        InitializeUncutGemIds();
    }

    private void FillCategoryItems(List<ApiCategory> categories, string categoryId, Category category)
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
            entry.Category = category;
            IdDictionary.Add(entry.Id, entry);
            NameDictionary.TryAdd(entry.Name ?? entry.Type ?? "", entry);
        }
    }

    private void InitializeUncutGemIds()
    {
        foreach (var item in IdDictionary)
        {
            switch (item.Value.Type)
            {
                case "Uncut Skill Gem":
                    UncutSkillGemId = item.Key;
                    break;
                case "Uncut Spirit Gem":
                    UncutSpiritGemId = item.Key;
                    break;
                case "Uncut Support Gem":
                    UncutSupportGemId = item.Key;
                    break;
            }
        }
    }
}
