using Sidekick.Apis.Poe2Scout.Categories.Models;
using Sidekick.Apis.Poe2Scout.Clients;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Extensions;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe2Scout.Categories;

public class ScoutCategoryProvider(
    IScoutClient scoutClient,
    ISettingsService settingsService,
    ICacheProvider cacheProvider) : IScoutCategoryProvider
{
    public async Task<List<ScoutCategory>> GetUniqueCategories()
    {
        var categories = await GetCategories();
        return categories?.UniqueCategories ?? [];
    }

    public async Task<List<ScoutCategory>> GetCurrencyCategories()
    {
        var categories = await GetCategories();
        return categories?.CurrencyCategories ?? [];
    }

    private async Task<ApiCategoriesResult?> GetCategories()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();
        var cacheKey = $"poe2scout.{game.GetValueAttribute()}.categories";

        return await cacheProvider.GetOrSet(cacheKey, Fetch, (result) => result.CurrencyCategories.Count > 0 && result.UniqueCategories.Count > 0);
    }

    private async Task<ApiCategoriesResult> Fetch()
    {
        return await scoutClient.Fetch<ApiCategoriesResult>("items/categories") ?? new();
    }

    public Uri GetUri(string? category)
    {
        return new Uri($"https://poe2scout.com/economy/{category}");
    }

}
