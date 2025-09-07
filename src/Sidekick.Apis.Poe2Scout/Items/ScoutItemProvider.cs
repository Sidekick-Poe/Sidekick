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
    public async Task<List<ScoutItem>> GetUniqueItems()
    {
        var categories = await categoryProvider.GetUniqueCategories();
        var items = await GetItems(categories, "unique");
        return items ?? [];
    }

    public async Task<List<ScoutItem>> GetCurrencyItems()
    {
        var categories = await categoryProvider.GetCurrencyCategories();
        var items = await GetItems(categories, "currency");
        return items ?? [];
    }

    private async Task<List<ScoutItem>?> GetItems(List<ScoutCategory> categories, string path)
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();
        var cacheKey = $"poe2scout.{game.GetValueAttribute()}.items.{path}";

        return await cacheProvider.GetOrSet(cacheKey, () => Fetch(categories, path), (result) => result.Count > 0);
    }

    private async Task<List<ScoutItem>> Fetch(List<ScoutCategory> categories, string path)
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

        return items;
    }

}
