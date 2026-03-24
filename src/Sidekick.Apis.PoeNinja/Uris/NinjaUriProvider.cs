using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Uris.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Items;
namespace Sidekick.Apis.PoeNinja.Uris;

public class NinjaUriProvider(
    INinjaClient ninjaClient,
    ICacheProvider cacheProvider,
    ISettingsService settingsService)
{
    public async Task<Uri?> GetDetailsUri(NinjaItemDefinition item)
    {
        string? detailsId = null;
        if (item.Exchange != null) detailsId = item.Exchange.Id;
        else if (item.Stash != null) detailsId = item.Stash.DetailsId;
        if (string.IsNullOrEmpty(detailsId)) return null;

        var game = await settingsService.GetGame();
        var gamePath = game == GameType.PathOfExile1 ? "" : "poe2/";
        var league = await GetLeague();
        return new Uri($"https://poe.ninja/{gamePath}economy/{league?.Url}/{item.Url}/{detailsId}");
    }

    private async Task<string> GetCacheKey()
    {
        var game = await settingsService.GetGame();
        return $"PoeNinjaIndexState_{game.GetValueAttribute()}";
    }

    public async Task<IndexStateLeague?> GetLeague()
    {
        var result = await GetResult();
        if (result == null) return null;

        var league = await settingsService.GetLeague();
        var line = result.EconomyLeagues.FirstOrDefault(x => x.Name == league);
        return line ?? null;

        async Task<bool> CheckCacheIsValid(IndexStateModel? model = null)
        {
            var lastUpdate = model?.LastUpdated ?? DateTimeOffset.MinValue;
            var isCacheTimeValid = DateTimeOffset.Now - lastUpdate <= TimeSpan.FromHours(12);
            if (isCacheTimeValid) return true;

            var cacheKey = await GetCacheKey();
            cacheProvider.Delete(cacheKey);
            return false;
        }

        async Task<IndexStateModel?> GetResult()
        {
            var cache = await GetOrUpdateCache();
            if (!await CheckCacheIsValid(cache))
            {
                return await GetOrUpdateCache();
            }

            return cache;
        }

        async Task<IndexStateModel?> GetOrUpdateCache()
        {
            var cacheKey = await GetCacheKey();
            return await cacheProvider.GetOrSet(cacheKey, async () => await Fetch(), x => x.EconomyLeagues.Count != 0);
        }

        async Task<IndexStateModel> Fetch()
        {
            var game = await settingsService.GetGame();
            var response = await ninjaClient.Fetch<IndexStateModel>(game, "data/index-state");
            if (response == null) return new();

            response.LastUpdated = DateTimeOffset.Now;
            return response;
        }
    }
}
