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

    public async Task<NinjaStash?> GetUniqueInfo(string? name, int links)
    {
        if (name == null) return null;

        var page = ninjaItemProvider.GetPage(name);
        if (page == null || page.SupportsExchange) return null;

        var result = await GetResult(page.Type);
        if (result == null) return null;

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

        var line = result.Lines
            .Where(x => x.Name == name)
            .OrderBy(x => x.GemLevel == gemLevel ? 0 : 1)
            .ThenBy(x => x.ChaosValue)
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
            .OrderBy(x => x.MapTier == mapTier ? 0 : 1)
            .ThenBy(x => x.ChaosValue)
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

        var line = result.Lines
            .Where(x => x.Name == grantText)
            .OrderByDescending(x =>
            {
                var validConditions = 0;
                if (x.Variant == $"{passiveCount} passives") validConditions++;
                if (x.LevelRequired == itemLevel) validConditions++;
                return validConditions;
            })
            .ThenBy(x => x.ChaosValue)
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
        var line = result.Lines
            .Where(x => x.Name == name)
            .OrderByDescending(x =>
            {
                var validConditions = 0;
                if (x.Variant != null && variants.Contains(x.Variant)) validConditions++;
                if (x.LevelRequired == itemLevel) validConditions++;
                return validConditions;
            })
            .ThenBy(x => x.ChaosValue)
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
        var gamePath = game == GameType.PathOfExile ? "poe1" : "poe2";
        return new Uri($"https://poe.ninja/{gamePath}/economy/{league}/{page.Url}/{line.DetailsId}");
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
        var isCacheTimeValid = DateTimeOffset.Now - lastUpdate <= TimeSpan.FromHours(1);
        if (isCacheTimeValid) return true;

        var cacheKey = await GetCacheKey(type);
        cacheProvider.Delete(cacheKey);
        return false;
    }

}
