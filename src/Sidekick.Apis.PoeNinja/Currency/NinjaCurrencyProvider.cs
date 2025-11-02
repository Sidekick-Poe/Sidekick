using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Currency.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.PoeNinja.Currency;

public class NinjaCurrencyProvider(
    INinjaClient ninjaClient,
    ISettingsService settingsService,
    ICacheProvider cacheProvider)
{
    private const string CacheKey = "PoeNinjaCurrency";

    public async Task<List<NinjaCurrency>?> GetPriceInfo(string? invariantId)
    {
        var result = await GetResult();
        if (result == null) return null;

        var line = result.Lines.FirstOrDefault(x => x.Id == invariantId);
        if (line == null) return null;

        return result.Core.Items.Select(x => new NinjaCurrency(line, result.Core, x.Id)).ToList();
    }

    private async Task<ApiOverviewResult?> GetResult()
    {
        await ClearCacheIfExpired();
        var result = await cacheProvider.GetOrSet(CacheKey, FetchResult, x=> x.Lines.Any());
        return result;
    }

    private async Task<ApiOverviewResult> FetchResult()
    {
        var result = await ninjaClient.Fetch<ApiOverviewResult>("economy/exchange/current/overview", new Dictionary<string, string?>()
        {
            {
                "type", "Currency"
            },
        });
        if (result == null) return new();

        await cacheProvider.Set(CacheKey, result);
        return result;
    }

    private async Task ClearCacheIfExpired()
    {
        var lastUpdate = await settingsService.GetDateTime(SettingKeys.PoeNinjaCurrencyLastUpdate);
        var isCacheTimeValid = lastUpdate.HasValue && DateTimeOffset.Now - lastUpdate <= TimeSpan.FromHours(1);
        if (isCacheTimeValid) return;

        cacheProvider.Delete(CacheKey);
        await settingsService.Set(SettingKeys.PoeNinjaCurrencyLastUpdate, DateTimeOffset.Now);
    }

}
