using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Exchange.Models;
using Sidekick.Apis.PoeNinja.IndexState;
using Sidekick.Common.Cache;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Ninja;
namespace Sidekick.Apis.PoeNinja.Exchange;

public class NinjaExchangeProvider(
    INinjaClient ninjaClient,
    ISettingsService settingsService,
    ICacheProvider cacheProvider,
    INinjaIndexStateProvider indexStateProvider) : INinjaExchangeProvider
{
    private async Task<string> GetCacheKey(string type)
    {
        var league = await settingsService.GetLeague();
        return $"PoeNinjaExchange_{league}_{type}";
    }

    public async Task<NinjaCurrency?> GetInfo(NinjaExchangeItem item)
    {
        var result = await GetResult(item.Page.Type);
        if (result?.Core == null) return null;

        var line = result.Lines.FirstOrDefault(x => x.Id == item.Id);

        // In some cases, the currency is not listed in the exchange overview, but is the primary currency.
        // This is the case for Path of Exile 1's Chaos Orb. It is the main comparison currency, but is absent from the lines.
        if (line == null && item.Page.Type == "Currency" && result.Core.Primary == item.Id)
        {
            line = new ApiLine()
            {
                Id = item.Id,
                PrimaryValue = 1,
            };
        }

        if (line == null) return null;

        return new NinjaCurrency(line, result)
        {
            DetailsUrl = await GetDetailsUri(item),
        };
    }

    private async Task<Uri> GetDetailsUri(NinjaExchangeItem item)
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

                var query = new Dictionary<string, string?>()
                {
                    {
                        "type", type
                    },
                };

                var response = await ninjaClient.Fetch<ApiOverviewResult>(game, "economy/exchange/current/overview", query);
                if (response == null) return new();

                response.LastUpdated = DateTimeOffset.Now;
                return response;
            }, x => x.Lines.Any());
        }
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
