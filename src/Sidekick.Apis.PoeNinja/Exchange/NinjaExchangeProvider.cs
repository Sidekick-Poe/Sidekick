using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Exchange.Models;
using Sidekick.Apis.PoeNinja.Items;
using Sidekick.Common.Cache;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.PoeNinja.Exchange;

public class NinjaExchangeProvider(
    INinjaClient ninjaClient,
    ISettingsService settingsService,
    INinjaItemProvider ninjaItemProvider,
    ICacheProvider cacheProvider) : INinjaExchangeProvider
{
    private async Task<string> GetCacheKey(string type)
    {
        var league = await settingsService.GetLeague();
        return $"PoeNinjaExchange_{league}_{type}";
    }

    public async Task<NinjaCurrency?> GetInfo(string? invariantId)
    {
        if (invariantId == null) return null;

        var page = ninjaItemProvider.GetPage(invariantId);
        if (page == null) return null;

        var result = await GetResult(page.Type);
        if (result?.Core == null) return null;

        var line = result.Lines.FirstOrDefault(x => x.Id == invariantId);

        // In some cases, the currency is not listed in the exchange overview, but is the primary currency.
        // This is the case for Path of Exile 1's Chaos Orb. It is the main comparison currency, but is absent from the lines.
        if (line == null && page.Type == "Currency" && result.Core.Primary == invariantId)
        {
            line = new ApiLine()
            {
                Id = invariantId,
                PrimaryValue = 1,
            };
        }

        if (line == null) return null;

        return new NinjaCurrency(line, result)
        {
            DetailsUrl = await GetDetailsUri(invariantId),
        };
    }

    private async Task<Uri?> GetDetailsUri(string? invariantId)
    {
        if (invariantId == null) return null;

        var page = ninjaItemProvider.GetPage(invariantId);
        if (page == null) return null;

        var league = await settingsService.GetLeague();
        var game = await settingsService.GetGame();
        var gamePath = game == GameType.PathOfExile ? "" : "poe2/";
        return new Uri($"https://poe.ninja/{gamePath}economy/{league?.ToLowerInvariant()}/{page.Url}/{invariantId}");
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

        var result = await ninjaClient.Fetch<ApiOverviewResult>(game, "economy/exchange/current/overview", query);
        if (result == null) return new();

        result.LastUpdated = DateTimeOffset.Now;
        return result;
    }

    private async Task<bool> CheckCacheIsValid(string type, ApiOverviewResult? result = null)
    {
        var lastUpdate = result?.LastUpdated ?? DateTimeOffset.MinValue;
        var isCacheTimeValid = DateTimeOffset.Now - lastUpdate <= TimeSpan.FromHours(1);
        if (isCacheTimeValid) return true;

        var cacheKey = await GetCacheKey(type);
        cacheProvider.Delete(cacheKey);
        return false;
    }

}
