using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Items;
using Sidekick.Apis.PoeNinja.Items.Models;
using Sidekick.Apis.PoeNinja.Stash.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.PoeNinja.Stash;

public class NinjaStashProvider(
    INinjaClient ninjaClient,
    ISettingsService settingsService,
    INinjaItemProvider ninjaItemProvider,
    ICacheProvider cacheProvider) : INinjaStashProvider
{
    private async Task<string> GetCacheKey(string type)
    {
        var league = await settingsService.GetLeague();
        return $"PoeNinjaStash_{league}_{type}";
    }

    public async Task<NinjaStash?> GetInfo(Item item)
    {
        if (item.Properties.Rarity == Rarity.Unique)
        {
            return await GetUniqueInfo(item.ApiInformation.InvariantName, item.Properties.GetMaximumNumberOfLinks());
        }

        if (item.Properties.ItemClass is ItemClass.ActiveGem or ItemClass.SupportGem)
        {
            return await GetGemInfo(item.ApiInformation.InvariantType, item.Properties.GemLevel);
        }

        if (item.Properties.MapTier != 0)
        {
            return await GetMapInfo(item.ApiInformation.InvariantType, item.Properties.MapTier);
        }

        if (item.Properties.ClusterJewelPassiveCount.HasValue && item.Properties.ClusterJewelPassiveCount != 0)
        {
            return await GetClusterInfo(item.Properties.ClusterJewelGrantText, item.Properties.ClusterJewelPassiveCount.Value, item.Properties.ItemLevel);
        }

        return await GetBaseTypeInfo(item.ApiInformation.InvariantType, item.Properties.ItemLevel, item.Properties.Influences);
    }

    public async Task<NinjaStash?> GetUniqueInfo(string? name, int links)
    {
        if (name == null) return null;

        var page = ninjaItemProvider.GetPage(name);
        if (page == null || page.SupportsExchange) return null;

        var result = await GetResult(page.Type);
        if (result == null) return null;

        if (links < 5) links = 0;

        var line = result.Lines
            .Where(x => x.Name == name)
            .OrderBy(x => x.Links == links ? 0 : 1)
            .ThenBy(x => x.ChaosValue)
            .FirstOrDefault();
        if (line == null) return null;

        return new NinjaStash(line, result)
        {
            DetailsUrl = await GetDetailsUri(page, line),
        };
    }

    public async Task<NinjaStash?> GetGemInfo(string? name, int gemLevel)
    {
        if (name == null) return null;

        var page = ninjaItemProvider.GetPage(name);
        if (page == null || page.SupportsExchange) return null;

        var result = await GetResult(page.Type);
        if (result == null) return null;

        if (gemLevel > 7 && gemLevel < 20) gemLevel = 1;

        var line = result.Lines
            .Where(x => x.Name == name)
            .Where(x => x.GemLevel == gemLevel)
            .OrderBy(x => x.ChaosValue)
            .FirstOrDefault();
        if (line == null) return null;

        return new NinjaStash(line, result)
        {
            DetailsUrl = await GetDetailsUri(page, line),
        };
    }

    public async Task<NinjaStash?> GetMapInfo(string? name, int mapTier)
    {
        if (name == null) return null;

        var page = ninjaItemProvider.GetPage(name);
        if (page == null || page.SupportsExchange) return null;

        var result = await GetResult(page.Type);
        if (result == null) return null;

        var line = result.Lines
            .Where(x => x.Name == name)
            .Where(x => x.MapTier == mapTier)
            .OrderBy(x => x.ChaosValue)
            .FirstOrDefault();
        if (line == null) return null;

        return new NinjaStash(line, result)
        {
            DetailsUrl = await GetDetailsUri(page, line),
        };
    }

    public async Task<NinjaStash?> GetClusterInfo(string? grantText, int passiveCount, int itemLevel)
    {
        if (grantText == null) return null;

        var page = ninjaItemProvider.GetPage(grantText);
        if (page == null || page.SupportsExchange) return null;

        var result = await GetResult(page.Type);
        if (result == null) return null;

        if (itemLevel < 50) itemLevel = 1;
        else if (itemLevel < 68) itemLevel = 50;
        else if (itemLevel < 75) itemLevel = 68;
        else if (itemLevel < 84) itemLevel = 75;
        else itemLevel = 84;

        var line = result.Lines
            .Where(x => x.Name == grantText)
            .Where(x => x.Variant == $"{passiveCount} passives")
            .Where(x => x.LevelRequired == itemLevel)
            .OrderBy(x => x.ChaosValue)
            .FirstOrDefault();
        if (line == null) return null;

        return new NinjaStash(line, result)
        {
            DetailsUrl = await GetDetailsUri(page, line),
        };
    }

    public async Task<NinjaStash?> GetBaseTypeInfo(string? name, int itemLevel, Influences influences)
    {
        if (name == null) return null;

        var page = ninjaItemProvider.GetPage(name);
        if (page == null || page.SupportsExchange) return null;

        var result = await GetResult(page.Type);
        if (result == null) return null;

        var variants = GetVariants().ToList();
        if (itemLevel > 86) itemLevel = 86;
        else if (itemLevel < 82) itemLevel = 0;

        var line = result.Lines
            .Where(x => x.Name == name)
            .Where(x => (x.Variant == null && variants.Count == 0) || (x.Variant != null && variants.Contains(x.Variant)))
            .Where(x => x.LevelRequired == itemLevel)
            .OrderBy(x => x.ChaosValue)
            .FirstOrDefault();
        if (line == null) return null;

        return new NinjaStash(line, result)
        {
            DetailsUrl = await GetDetailsUri(page, line),
        };

        IEnumerable<string> GetVariants()
        {
            List<string> influenceNames = [];
            if (influences.Crusader) influenceNames.Add("Crusader");
            if (influences.Warlord) influenceNames.Add("Warlord");
            if (influences.Hunter) influenceNames.Add("Hunter");
            if (influences.Redeemer) influenceNames.Add("Redeemer");
            if (influences.Shaper) influenceNames.Add("Shaper");
            if (influences.Elder) influenceNames.Add("Elder");

            // Generate all permutations of the influences list
            foreach (var permutation in GetPermutations(influenceNames))
            {
                yield return string.Join("/", permutation);
            }

            yield break;

            IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> enumerable)
            {
                var list = enumerable.ToList();
                if (!list.Any())
                {
                    yield return [];
                    yield break;
                }

                foreach (var element in list)
                {
                    var remainingList = list.Where(x => !x!.Equals(element));
                    foreach (var permutation in GetPermutations(remainingList))
                    {
                        yield return new[]
                        {
                            element,
                        }.Concat(permutation);
                    }
                }
            }
        }
    }

    private async Task<Uri?> GetDetailsUri(NinjaPage page, ApiLine line)
    {
        if (line.DetailsId == null) return null;

        var league = await settingsService.GetLeague();
        var game = await settingsService.GetGame();
        var gamePath = game == GameType.PathOfExile ? "" : "poe2/";
        return new Uri($"https://poe.ninja/{gamePath}economy/{league?.ToLowerInvariant()}/{page.Url}/{line.DetailsId}");
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

    public async Task<ApiOverviewResult> FetchOverview(GameType game, string type)
    {
        var result = await ninjaClient.Fetch<ApiOverviewResult>(game, "economy/stash/current/item/overview", new Dictionary<string, string?>()
        {
            {
                "type", type
            },
        });
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
