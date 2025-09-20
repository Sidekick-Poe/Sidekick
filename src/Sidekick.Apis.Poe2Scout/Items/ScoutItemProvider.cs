using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe2Scout.Categories;
using Sidekick.Apis.Poe2Scout.Categories.Models;
using Sidekick.Apis.Poe2Scout.Clients;
using Sidekick.Apis.Poe2Scout.Items.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Extensions;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe2Scout.Items;

public class ScoutItemProvider(
    ISettingsService settingsService,
    IScoutCategoryProvider categoryProvider,
    ICacheProvider cacheProvider,
    IScoutClient scoutClient) : IScoutItemProvider
{
    private List<ScoutItem>? Items { get; set; }

    public async Task<ScoutItem?> GetItem(string? name, string? type)
    {
        name ??= type;
        if (string.IsNullOrEmpty(name)) return null;

        var items = await GetOrFetchItems();
        var item = items.FirstOrDefault(x => !string.IsNullOrEmpty(x.Name) && x.Name == name);
        item ??= items.FirstOrDefault(x => !string.IsNullOrEmpty(x.Type) && x.Type == name);
        item ??= items.FirstOrDefault(x => !string.IsNullOrEmpty(x.Text) && x.Text == name);

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

    private async Task<List<ScoutItem>> GetOrFetchItemsByCategories()
    {
        var game = await settingsService.GetGame();
        var cacheKey = $"poe2scout.{game.GetValueAttribute()}.items";

        var uniqueCategories = await categoryProvider.GetUniqueCategories();
        var currencyCategories = await categoryProvider.GetCurrencyCategories();

        return
        [
            ..await cacheProvider.GetOrSet(cacheKey + ".unique", () => FetchItemsByCategories(uniqueCategories, "unique", false), (result) => result.Count > 0) ?? [],
            ..await cacheProvider.GetOrSet(cacheKey + ".currency", () => FetchItemsByCategories(currencyCategories, "currency", true), (result) => result.Count > 0) ?? [],
        ];
    }

    private async Task<List<ScoutItem>> FetchItemsByCategories(List<ScoutCategory> categories, string path, bool isCurrency)
    {
        var items = new List<ScoutItem>();
        foreach (var category in categories)
        {
            var page = 1;
            while (true)
            {
                var result = await scoutClient.Fetch<ApiItemsResult>($"items/{path}/{category.ApiId}", new Dictionary<string, string?>
                {
                    {
                        "page", page.ToString()
                    },
                    {
                        "perPage", "250"
                    },
                });
                if (result == null) break;

                items.AddRange(result.Items);

                if (result.Pages == result.CurrentPage) break;
                page++;
            }
        }

        items.ForEach(x => x.IsCurrency = isCurrency);

        return items;
    }
}
