using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.IndexState;
using Sidekick.Apis.PoeNinja.Stash.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Settings;
using Sidekick.Data.Items;
using Sidekick.Data.Ninja;
namespace Sidekick.Apis.PoeNinja.Stash;

public class NinjaStashProvider(
    INinjaClient ninjaClient,
    ISettingsService settingsService,
    ICacheProvider cacheProvider,
    INinjaIndexStateProvider indexStateProvider) : INinjaStashProvider
{
    private async Task<string> GetCacheKey(string type)
    {
        var league = await settingsService.GetLeague();
        return $"PoeNinjaStash_{league}_{type}";
    }

    public async Task<NinjaStash?> GetInfo(NinjaStashItem item)
    {
        var result = await GetResult(item.Page.Type);
        if (result == null) return null;

        var line = result.Lines.FirstOrDefault(x => x.DetailsId == item.DetailsId);

        if (line == null) return null;

        return new NinjaStash(line, result)
        {
            DetailsUrl = await GetDetailsUri(item),
        };
    }

    private async Task<Uri?> GetDetailsUri(NinjaStashItem item)
    {
        var game = await settingsService.GetGame();
        var gamePath = game == GameType.PathOfExile1 ? "" : "poe2/";
        var leagueIndexState = await indexStateProvider.GetLeague();
        return new Uri($"https://poe.ninja/{gamePath}economy/{leagueIndexState?.Url}/{item.Page.Url}/{item.DetailsId}");
    }

    private async Task<ApiOverviewResult?> GetResult(string type)
    {
        var result = await GetOrUpdateCache();
        if (!await CheckCacheIsValid(type, result))
        {
            result = await GetOrUpdateCache();
        }

        return result;

        async Task<ApiOverviewResult?> GetOrUpdateCache()
        {
            var cacheKey = await GetCacheKey(type);
            return await cacheProvider.GetOrSet(cacheKey, async () =>
            {
                var game = await settingsService.GetGame();
                return await FetchOverview(game, type);
            }, x => x.Lines.Any());
        }
    }

    public async Task<ApiOverviewResult> FetchOverview(GameType game, string type, string? leagueOverride = null)
    {
        var query = new Dictionary<string, string?>()
        {
            {
                "type", type
            },
        };
        if (leagueOverride != null)
        {
            query.Add("league", leagueOverride);
        }

        var result = await ninjaClient.Fetch<ApiOverviewResult>(game, "economy/stash/current/item/overview", query);
        if (result == null) return new();

        result.LastUpdated = DateTimeOffset.Now;
        return result;
    }

    private async Task<bool> CheckCacheIsValid(string type, ApiOverviewResult? result = null)
    {
        var lastUpdate = result?.LastUpdated ?? DateTimeOffset.MinValue;
        var isCacheTimeValid = DateTimeOffset.Now - lastUpdate <= TimeSpan.FromHours(2);
        if (isCacheTimeValid) return true;

        var cacheKey = await GetCacheKey(type);
        cacheProvider.Delete(cacheKey);
        return false;
    }

}
