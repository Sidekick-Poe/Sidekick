using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe2Scout.Categories;
using Sidekick.Apis.Poe2Scout.Clients;
using Sidekick.Apis.Poe2Scout.Items.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Items;

namespace Sidekick.Apis.Poe2Scout.Items;

public class ScoutItemProvider(
    ISettingsService settingsService,
    IScoutCategoryProvider categoryProvider,
    ICacheProvider cacheProvider,
    IScoutClient scoutClient) : IScoutItemProvider
{
    private List<ScoutItem>? Items { get; set; }

    public Task<ScoutItem?> GetItem(ItemDefinition itemDefinition)
    {
        var text = itemDefinition.TradeItem?.Name;
        text ??= itemDefinition.TradeItem?.Text;
        text ??= itemDefinition.TradeItem?.Type;
        if (string.IsNullOrEmpty(text)) return Task.FromResult<ScoutItem?>(null);

        return GetItem(text);
    }

    public async Task<ScoutItem?> GetItem(string text)
    {
        var game = await settingsService.GetGame();
        if (game == GameType.PathOfExile1) return null;

        if (string.IsNullOrEmpty(text)) return null;

        var items = await GetOrFetchItems();
        var item = items.FirstOrDefault(x => !string.IsNullOrEmpty(x.Name) && x.Name == text);
        item ??= items.FirstOrDefault(x => !string.IsNullOrEmpty(x.Type) && x.Type == text);
        item ??= items.FirstOrDefault(x => !string.IsNullOrEmpty(x.Text) && x.Text == text);

        return item;
    }

    private async Task<List<ScoutItem>> GetOrFetchItems()
    {
        if (Items != null) return Items;

        var game = await settingsService.GetGame();
        var cacheKey = $"poe2scout.{game.GetValueAttribute()}.items";

        Items = await cacheProvider.GetOrSet(cacheKey, FetchAllItems, (result) => result.Count > 0);
        return Items ?? [];
    }

    private async Task<List<ScoutItem>> FetchAllItems()
    {
        var result = await scoutClient.Fetch<List<ScoutItem>>($"items");
        if (result == null) return [];

        var currencyCategories = await categoryProvider.GetCurrencyCategories();
        var currencyCategoryNames = currencyCategories.Select(x => x.ApiId).ToList();
        result.ForEach(x => x.IsCurrency = currencyCategoryNames.Contains(x.CategoryApiId));

        return result;
    }
}
