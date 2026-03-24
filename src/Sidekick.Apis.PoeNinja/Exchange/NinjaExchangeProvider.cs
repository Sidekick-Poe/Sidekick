using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Exchange.Models;
using Sidekick.Apis.PoeNinja.Uris;
using Sidekick.Common.Cache;
using Sidekick.Common.Settings;
using Sidekick.Data.Items;
namespace Sidekick.Apis.PoeNinja.Exchange;

public class NinjaExchangeProvider(
    INinjaClient ninjaClient,
    ISettingsService settingsService,
    ICacheProvider cacheProvider,
    NinjaUriProvider ninjaUriProvider) : INinjaExchangeProvider
{
    private async Task<string> GetCacheKey(string type)
    {
        var league = await settingsService.GetLeague();
        return $"PoeNinjaExchange_{league}_{type}";
    }

    public async Task<NinjaCurrency?> GetInfo(ItemDefinition item)
    {
        var bestMatch = FindBestMatch();
        if (bestMatch?.Exchange == null) return null;

        var result = await GetExchangeResult(bestMatch.Type);
        if (result?.Core == null) return null;

        var line = result.Lines.FirstOrDefault(x => x.Id == bestMatch.Exchange.Id);

        // In some cases, the currency is not listed in the exchange overview, but is the primary currency.
        // This is the case for Path of Exile 1's Chaos Orb. It is the main comparison currency, but is absent from the lines.
        if (line == null && bestMatch.Type == "Currency" && result.Core.Primary == bestMatch.Exchange.Id)
        {
            line = new NinjaExchangeLine()
            {
                Id = bestMatch.Exchange.Id,
                PrimaryValue = 1,
            };
        }

        if (line == null) return null;

        return new NinjaCurrency(line, result)
        {
            DetailsUrl = await ninjaUriProvider.GetDetailsUri(bestMatch),
        };

        NinjaItemDefinition? FindBestMatch()
        {
            return item.NinjaItems?.FirstOrDefault();
        }
    }

    private async Task<NinjaExchangeOverview?> GetExchangeResult(string type)
    {
        var result = await GetOrUpdateCache();
        if (!await CheckCacheIsValid(type, result))
        {
            result = await GetOrUpdateCache();
        }

        return result;

        async Task<NinjaExchangeOverview?> GetOrUpdateCache()
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

                var response = await ninjaClient.Fetch<NinjaExchangeOverview>(game, "economy/exchange/current/overview", query);
                if (response == null) return new();

                response.LastUpdated = DateTimeOffset.Now;
                return response;
            }, x => x.Lines.Any());
        }
    }

    private async Task<bool> CheckCacheIsValid(string type, NinjaExchangeOverview? result = null)
    {
        var lastUpdate = result?.LastUpdated ?? DateTimeOffset.MinValue;
        var isCacheTimeValid = DateTimeOffset.Now - lastUpdate <= TimeSpan.FromHours(1);
        if (isCacheTimeValid) return true;

        var cacheKey = await GetCacheKey(type);
        cacheProvider.Delete(cacheKey);
        return false;
    }

}
