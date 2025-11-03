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
    private string GetCacheKey(string type) => $"PoeNinjaExchange_{type}";

    public async Task<NinjaCurrency?> GetCurrencyInfo(string? invariantId)
    {
        if (invariantId == null) return null;

        var page = ninjaItemProvider.GetPage(invariantId);
        if (page == null) return null;

        var result = await GetResult(page.Type);
        if (result?.Core == null) return null;

        var line = result.Lines.FirstOrDefault(x => x.Id == invariantId);
        if (line == null) return null;

        return new NinjaCurrency(line, result);
    }

    public async Task<Uri?> GetDetailsUri(string? invariantId)
    {
        if (invariantId == null) return null;

        var page = ninjaItemProvider.GetPage(invariantId);
        if (page == null) return null;

        var league = await settingsService.GetLeague();
        var game = await settingsService.GetGame();
        var gamePath = game == GameType.PathOfExile ? "poe1" : "poe2";
        return new Uri($"https://poe.ninja/{gamePath}/economy/{league}/{page.Url}/{invariantId}");
    }

    private async Task<ApiOverviewResult?> GetResult(string type)
    {
        var result = await GetOrUpdateCache();
        if (!CheckCacheIsValid(type, result))
        {
            result = await GetOrUpdateCache();
        }

        return result;

        async Task<ApiOverviewResult?> GetOrUpdateCache()
        {
            return await cacheProvider.GetOrSet(GetCacheKey(type), async () =>
            {
                var game = await settingsService.GetGame();
                return await FetchOverview(game, type);
            }, x => x.Lines.Any());
        }
    }

    public async Task<ApiOverviewResult> FetchOverview(GameType game, string type)
    {
        var result = await ninjaClient.Fetch<ApiOverviewResult>(game, "economy/exchange/current/overview", new Dictionary<string, string?>()
        {
            {
                "type", type
            },
        });
        if (result == null) return new();

        result.LastUpdated = DateTimeOffset.Now;
        return result;
    }

    private bool CheckCacheIsValid(string type, ApiOverviewResult? result = null)
    {
        var lastUpdate = result?.LastUpdated ?? DateTimeOffset.MinValue;
        var isCacheTimeValid = DateTimeOffset.Now - lastUpdate <= TimeSpan.FromHours(1);
        if (isCacheTimeValid) return true;

        cacheProvider.Delete(GetCacheKey(type));
        return false;
    }

}
