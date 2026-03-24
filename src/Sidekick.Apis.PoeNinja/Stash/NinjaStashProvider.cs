using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Items.Models;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Stash.Models;
using Sidekick.Apis.PoeNinja.Uris;
using Sidekick.Common.Cache;
using Sidekick.Common.Settings;
using Sidekick.Data.Items;
using Sidekick.Data.Stats;

namespace Sidekick.Apis.PoeNinja.Stash;

public class NinjaStashProvider(
    INinjaClient ninjaClient,
    ISettingsService settingsService,
    ICacheProvider cacheProvider,
    NinjaUriProvider ninjaUriProvider) : INinjaStashProvider
{
    private async Task<string> GetCacheKey(string type)
    {
        var league = await settingsService.GetLeague();
        return $"PoeNinjaStash_{league}_{type}";
    }

    public async Task<NinjaStash?> GetInfo(Item item)
    {
        var stats = (
            from stat in item.Stats
            from definition in stat.Definitions
            from tradeStat in definition.TradeStats
            select new CompareStat(stat.Category,
                                   tradeStat.Id,
                                   stat.AverageValue,
                                   tradeStat.Option?.Id.ToString())
            ).ToList();

        if (item.Properties.Rarity == Rarity.Unique)
        {
            return await GetUniqueInfo(item.Invariant,
                                       item.Properties.Foulborn,
                                       item.Properties.GetMaximumNumberOfLinks(),
                                       stats);
        }

        if (item.Properties.MapTier > 0 || item.Properties.ItemClass == ItemClass.Map)
        {
            return await GetMapInfo(item.Invariant,
                                    item.Properties.MapTier);
        }

        if (item.Properties.GemLevel > 0)
        {
            return await GetGemInfo(item.Invariant,
                                    item.Properties.Corrupted,
                                    item.Properties.GemLevel,
                                    item.Properties.Quality);
        }

        if (IsClusterJewel(item.Invariant))
        {
            return await GetClusterJewelInfo(item.Invariant,
                                             item.Properties.ItemLevel,
                                             stats);

        }

        return await GetBaseTypeInfo(item.Invariant,
                                     item.Properties.ItemLevel,
                                     item.Properties.Influences);
    }

    public async Task<NinjaStash?> GetInfo(ItemDefinition item, ApiItem apiItem)
    {
        // TODO #1061 Unsupported due to foulborn modifiers. We would need to create some logic to get the trade stat id from the ApiItem. This isn't something that is done currently.
        if (apiItem.Rarity == Rarity.Unique) return null;

        if (apiItem.GemLevel > 0)
        {
            // TODO #1060 Unsupported due to transfigured gems being parsed wrong.
            return null;
            return await GetGemInfo(item,
                                    apiItem.Corrupted,
                                    apiItem.GemLevel.Value,
                                    apiItem.Quality.GetValueOrDefault());
        }

        if (apiItem.MapTier > 0)
        {
            return await GetMapInfo(item,
                                    apiItem.MapTier.Value);
        }

        return await GetBaseTypeInfo(item,
                                     apiItem.ItemLevel,
                                     apiItem.Influences);
    }

    private async Task<NinjaStash?> GetUniqueInfo(ItemDefinition item, bool foulborn, int? links, List<CompareStat>? stats)
    {
        var bestMatch = FindBestMatch();
        return await BuildResult(bestMatch);

        NinjaItemDefinition? FindBestMatch()
        {
            if (item.NinjaItems == null) return null;

            if (links < 5) links = 0;
            if (stats != null)
            {
                stats = stats.Where(x => x.Category == StatCategory.Mutated).ToList();
                if (stats.Count == 0) stats = null;
            }

            return item.NinjaItems
                .Where(x => x.Stash != null)
                .Where(x => x.Stash!.Foulborn.GetValueOrDefault() == foulborn)
                .Where(x => x.Stash!.Links.GetValueOrDefault() == links.GetValueOrDefault())
                .FirstOrDefault(ninjaDefinition => ValidateNinjaStats(stats, ninjaDefinition));
        }
    }

    private async Task<NinjaStash?> GetMapInfo(ItemDefinition item, int mapTier)
    {
        var bestMatch = FindBestMatch();
        return await BuildResult(bestMatch);

        NinjaItemDefinition? FindBestMatch()
        {
            if (item.NinjaItems == null) return null;
            if (string.IsNullOrEmpty(item.BaseItem?.Name)) return null;

            var name = item.BaseItem.Name;
            if (name == "Map") name = $"Map (Tier {mapTier})";
            if (name == "Blighted Map") name = $"Blighted Map (Tier {mapTier})";
            if (name == "Blight-ravaged Map") name = $"Blight-ravaged Map (Tier {mapTier})";

            return item.NinjaItems
                .Where(x => x.Stash != null)
                .FirstOrDefault(x => x.Stash!.Name == name);
        }
    }

    private async Task<NinjaStash?> GetGemInfo(ItemDefinition item, bool corrupted, int gemLevel, int gemQuality)
    {
        var bestMatch = FindBestMatch();
        return await BuildResult(bestMatch);

        NinjaItemDefinition? FindBestMatch()
        {
            if (item.NinjaItems == null) return null;
            var text = item.TradeItem?.Text;
            text ??= item.BaseItem?.Name;
            if (string.IsNullOrEmpty(text)) return null;

            gemLevel = gemLevel switch
            {
                > 7 and < 20 => 1,
                _ => gemLevel
            };

            gemQuality = gemQuality switch
            {
                < 20 => 0,
                < 23 => 20,
                _ => 23
            };

            return item.NinjaItems
                .Where(x => x.Stash != null)
                .Where(x => x.Stash!.Name == text)
                .Where(x => x.Stash!.GemLevel.GetValueOrDefault() == gemLevel)
                .Where(x => x.Stash!.GemQuality.GetValueOrDefault() == gemQuality)
                .FirstOrDefault(x => x.Stash!.Corrupted.GetValueOrDefault() == corrupted);
        }
    }

    private bool IsClusterJewel(ItemDefinition item)
    {
        if (item.NinjaItems == null) return false;
        return item.BaseItem?.Name is "Small Cluster Jewel" or "Medium Cluster Jewel" or "Large Cluster Jewel";
    }

    private async Task<NinjaStash?> GetClusterJewelInfo(ItemDefinition item, int itemLevel, List<CompareStat>? stats)
    {
        var bestMatch = FindBestMatch();
        return await BuildResult(bestMatch);

        NinjaItemDefinition? FindBestMatch()
        {
            if (!IsClusterJewel(item)) return null;

            if (stats != null)
            {
                stats = stats
                    .Where(x => x.Category == StatCategory.Enchant)
                    .Where(x => x.Id.StartsWith("enchant."))
                    .ToList();
                if (stats.Count == 0) stats = null;
            }

            itemLevel = itemLevel switch
            {
                < 50 => 1,
                < 68 => 50,
                < 75 => 68,
                < 84 => 75,
                _ => 84,
            };

            return item.NinjaItems!
                .Where(x => x.Stash != null)
                .Where(x => x.Stash!.ItemLevel.GetValueOrDefault() == itemLevel)
                .FirstOrDefault(ninjaDefinition => ValidateNinjaStats(stats, ninjaDefinition));
        }
    }

    private async Task<NinjaStash?> GetBaseTypeInfo(ItemDefinition item, int itemLevel, Influences influences)
    {
        var bestMatch = FindBestMatch();
        return await BuildResult(bestMatch);

        NinjaItemDefinition? FindBestMatch()
        {
            if (item.NinjaItems == null) return null;
            if (string.IsNullOrEmpty(item.BaseItem?.Name)) return null;

            var variants = GetVariants().ToList();
            itemLevel = itemLevel switch
            {
                > 86 => 86,
                < 82 => 0,
                _ => itemLevel
            };

            itemLevel = itemLevel switch
            {
                >= 86 => 86,
                >= 85 => 85,
                >= 84 => 84,
                >= 83 => 83,
                >= 82 => 82,
                _ => 0,
            };

            if (itemLevel == 0) return null;

            return item.NinjaItems
                .Where(x => x.Stash != null)
                .Where(x => x.Stash!.Name == item.BaseItem.Name)
                .Where(x => x.Stash!.ItemLevel.GetValueOrDefault() == itemLevel)
                .FirstOrDefault(x => (x.Stash!.Variant == null && variants.Count == 0) || (x.Stash!.Variant != null && variants.Contains(x.Stash!.Variant)));
        }

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
                var values = permutation.ToList();
                if (values.Count != 0) yield return string.Join("/", values);
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

    private record CompareStat(StatCategory Category, string Id, double? Value, string? Option);

    private static bool ValidateNinjaStats(List<CompareStat>? stats, NinjaItemDefinition ninjaDefinition)
    {
        if (stats == null) return ninjaDefinition.Stash!.Stats == null;
        if (ninjaDefinition.Stash!.Stats?.Count != stats.Count) return false;

        foreach (var expectedStat in ninjaDefinition.Stash.Stats)
        {
            var foundStat = stats.FirstOrDefault(stat => stat.Id == expectedStat.Id);

            if (foundStat == null) return false;
            if (!string.IsNullOrEmpty(expectedStat.Option) &&
                foundStat.Option != expectedStat.Option) return false;

            if (expectedStat.Value != null && expectedStat.Value != 0 &&
                (int)Math.Round(foundStat.Value ?? 0) != expectedStat.Value) return false;
        }

        return true;
    }

    private async Task<NinjaStash?> BuildResult(NinjaItemDefinition? item)
    {
        if (item?.Stash == null) return null;

        var result = await GetResult(item.Type);
        if (result == null) return null;

        var line = result.Lines.FirstOrDefault(x => x.DetailsId == item.Stash.DetailsId);
        if (line == null) return null;

        return new NinjaStash(line, result)
        {
            DetailsUrl = await ninjaUriProvider.GetDetailsUri(item),
        };
    }

    private async Task<NinjaStashOverview?> GetResult(string type)
    {
        var result = await GetOrUpdateCache();
        if (!await CheckCacheIsValid(type, result))
        {
            result = await GetOrUpdateCache();
        }

        return result;

        async Task<NinjaStashOverview?> GetOrUpdateCache()
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

                var response = await ninjaClient.Fetch<NinjaStashOverview>(game, "economy/stash/current/item/overview", query);
                if (response == null) return new();

                response.LastUpdated = DateTimeOffset.Now;
                return response;
            }, x => x.Lines.Any());
        }
    }

    private async Task<bool> CheckCacheIsValid(string type, NinjaStashOverview? result = null)
    {
        var lastUpdate = result?.LastUpdated ?? DateTimeOffset.MinValue;
        var isCacheTimeValid = DateTimeOffset.Now - lastUpdate <= TimeSpan.FromHours(2);
        if (isCacheTimeValid) return true;

        var cacheKey = await GetCacheKey(type);
        cacheProvider.Delete(cacheKey);
        return false;
    }

}
