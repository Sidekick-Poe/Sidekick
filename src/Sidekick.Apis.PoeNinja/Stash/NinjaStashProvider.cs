using Sidekick.Apis.Poe.Trade.Parser.Stats;
using Sidekick.Apis.Poe.Trade.Trade.Models;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Stash.Models;
using Sidekick.Apis.PoeNinja.Uris;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;
using Sidekick.Data.Extensions;
using Sidekick.Data.ItemClasses;
using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Items;
using Sidekick.Data.Stats;

namespace Sidekick.Apis.PoeNinja.Stash;

public class NinjaStashProvider(
    INinjaClient ninjaClient,
    ISettingsService settingsService,
    ICacheProvider cacheProvider,
    NinjaUriProvider ninjaUriProvider,
    IStatParser statParser) : INinjaStashProvider
{
    private static readonly List<string> IgnoreStatTexts =
    [
        "# Added Passive Skills are Jewel Sockets",
        "Area is influenced by The Shaper",
        "Area is influenced by The Elder",
    ];

    private async Task<string> GetCacheKey(string type)
    {
        var league = await settingsService.GetLeague();
        return $"PoeNinjaStash_{league}_{type}";
    }

    public async Task<List<NinjaStash>> GetInfo(Item item)
    {
        if (item.Properties.Rarity == Rarity.Unique)
        {
            return await GetUniqueInfo(item.Invariant,
                                       item.Properties.Foulborn,
                                       item.Properties.GetMaximumNumberOfLinks(),
                                       item.Stats);
        }

        if (item.Properties.MapTier > 0 || item.ItemClass.Type == ItemClass.Map)
        {
            return await GetMapInfo(item.Invariant.NinjaItems,
                                    item.Invariant.BaseItem?.Name,
                                    item.Properties.MapTier,
                                    item.Stats);
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
                                             item.Stats);

        }

        if (item.Invariant.TradeItem?.Category == "monster")
        {
            return await GetBeastInfo(item.Invariant);
        }

        return await GetBaseTypeInfo(item.Invariant,
                                     item.Properties.ItemLevel,
                                     item.Properties.Influences);
    }

    public async Task<List<NinjaStash>> GetInfo(ItemDefinition item, ApiItem apiItem)
    {
        var stats = apiItem.MutatedMods.Select(x => statParser.ParseInvariant($"{x} ({StatCategory.Mutated.GetValueAttribute()})")!).ToList();
        stats.AddRange(apiItem.EnchantMods.Select(x => statParser.ParseInvariant($"{x} ({StatCategory.Enchant.GetValueAttribute()})")!).ToList());
        stats.AddRange(apiItem.ImplicitMods.Select(x => statParser.ParseInvariant($"{x} ({StatCategory.Implicit.GetValueAttribute()})")!).ToList());
        stats = stats.Where(x => x != null!).ToList();

        if (apiItem.Rarity == Rarity.Unique)
        {
            return await GetUniqueInfo(item,
                                       apiItem.Mutated,
                                       apiItem.MaxLinks,
                                       stats);
        }

        if (apiItem.GemLevel > 0)
        {
            return await GetGemInfo(item,
                                    apiItem.Corrupted,
                                    apiItem.GemLevel.Value,
                                    apiItem.Quality.GetValueOrDefault());
        }

        if (apiItem.MapTier > 0 || item.TradeItem?.Category == "map")
        {
            return await GetMapInfo(item.NinjaItems,
                                    apiItem.Type,
                                    apiItem.MapTier,
                                    stats);
        }

        if (IsClusterJewel(item))
        {
            return await GetClusterJewelInfo(item,
                                             apiItem.ItemLevel,
                                             stats);

        }

        if (item.TradeItem?.Category == "monster")
        {
            return await GetBeastInfo(item);
        }

        return await GetBaseTypeInfo(item,
                                     apiItem.ItemLevel,
                                     apiItem.Influences);
    }

    private async Task<List<NinjaStash>> GetUniqueInfo(ItemDefinition item, bool foulborn, int? links, List<Stat>? stats)
    {
        var matches = FindMatches();
        return await BuildResult(matches);

        List<NinjaItemDefinition> FindMatches()
        {
            if (item.NinjaItems == null) return [];

            if (links < 5) links = 0;

            return item.NinjaItems
                .Where(x => x.Stash != null)
                .Where(x => x.Stash!.Foulborn.GetValueOrDefault() == foulborn)
                .Where(x => x.Stash!.Links.GetValueOrDefault() == links.GetValueOrDefault())
                .Where(x => ValidateNinjaStats(stats, StatCategory.Mutated, x))
                .ToList();
        }
    }

    private async Task<List<NinjaStash>> GetMapInfo(List<NinjaItemDefinition>? ninjaItems, string? type, int? mapTier, List<Stat>? stats)
    {
        var matches = FindMatches();
        return await BuildResult(matches);

        List<NinjaItemDefinition> FindMatches()
        {
            if (ninjaItems == null) return [];
            if (string.IsNullOrEmpty(type)) return [];

            if (stats != null && stats.Any(x => x.Category == StatCategory.Implicit))
            {
                var statsResults = ninjaItems
                    .Where(x => x.Stash != null)
                    .Where(x => ValidateNinjaStats(stats, StatCategory.Implicit, x))
                    .ToList();
                if (statsResults.Count > 0) return statsResults;
            }

            if (mapTier.HasValue)
            {
                if (type == "Map") type = $"Map (Tier {mapTier})";
                if (type == "Blighted Map") type = $"Blighted Map (Tier {mapTier})";
                if (type == "Blight-ravaged Map") type = $"Blight-ravaged Map (Tier {mapTier})";
            }

            return ninjaItems
                .Where(x => x.Stash != null)
                .Where(x => x.Stash!.Name == type)
                .ToList();
        }
    }

    private async Task<List<NinjaStash>> GetGemInfo(ItemDefinition item, bool corrupted, int gemLevel, int gemQuality)
    {
        var matches = FindMatches();
        return await BuildResult(matches);

        List<NinjaItemDefinition> FindMatches()
        {
            if (item.NinjaItems == null) return [];
            var text = item.TradeItem?.Text;
            text ??= item.BaseItem?.Name;
            if (string.IsNullOrEmpty(text)) return [];

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
                .Where(x => x.Stash!.Corrupted.GetValueOrDefault() == corrupted)
                .ToList();
        }
    }

    private bool IsClusterJewel(ItemDefinition item)
    {
        if (item.NinjaItems == null) return false;
        return item.BaseItem?.Name is "Small Cluster Jewel" or "Medium Cluster Jewel" or "Large Cluster Jewel";
    }

    private async Task<List<NinjaStash>> GetClusterJewelInfo(ItemDefinition item, int itemLevel, List<Stat>? stats)
    {
        var matches = FindMatches();
        return await BuildResult(matches);

        List<NinjaItemDefinition> FindMatches()
        {
            if (!IsClusterJewel(item)) return [];

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
                .Where(x => ValidateNinjaStats(stats, StatCategory.Enchant, x))
                .ToList();
        }
    }

    private async Task<List<NinjaStash>> GetBaseTypeInfo(ItemDefinition item, int itemLevel, Influences influences)
    {
        var matches = FindMatches();
        return await BuildResult(matches);

        List<NinjaItemDefinition> FindMatches()
        {
            if (item.NinjaItems == null) return [];
            if (string.IsNullOrEmpty(item.BaseItem?.Name)) return [];

            var variants = GetVariants().ToList();

            itemLevel = itemLevel switch
            {
                >= 86 => 86,
                >= 85 => 85,
                >= 84 => 84,
                >= 83 => 83,
                >= 82 => 82,
                _ => 0,
            };

            if (itemLevel == 0) return [];

            return item.NinjaItems
                .Where(x => x.Stash != null)
                .Where(x => x.Stash!.Name == item.BaseItem.Name)
                .Where(x => x.Stash!.ItemLevel.GetValueOrDefault() == itemLevel)
                .Where(x => (x.Stash!.Variant == null && variants.Count == 0) || (x.Stash!.Variant != null && variants.Contains(x.Stash!.Variant)))
                .ToList();
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

    private async Task<List<NinjaStash>> GetBeastInfo(ItemDefinition item)
    {
        var matches = FindMatches();
        return await BuildResult(matches);

        List<NinjaItemDefinition> FindMatches()
        {
            if (item.NinjaItems == null) return [];
            if (string.IsNullOrEmpty(item.TradeItem?.Type)) return [];

            return item.NinjaItems
                .Where(x => x.Stash != null)
                .Where(x => x.Stash!.Name == item.TradeItem?.Type)
                .ToList();
        }
    }

    private static bool ValidateNinjaStats(List<Stat>? itemStats, StatCategory statCategory, NinjaItemDefinition ninjaDefinition)
    {
        var statStartsWith = statCategory.GetValueAttribute();
        if (statCategory == StatCategory.Mutated) statStartsWith = "explicit";

        var stats = (
            from stat in itemStats
            from definition in stat.Definitions.Where(x => x.TradeStats != null)
            from tradeStat in definition.TradeStats
            where stat.Category == statCategory && tradeStat.Id.StartsWith(statStartsWith)
            where !IgnoreStatTexts.Contains(tradeStat.Text)
            select new
            {
                Id = tradeStatId.GetStatOption() != null ? $"{tradeStatId.GetStatId()}#{tradeStatId.GetStatOption()}" : tradeStatId.GetStatId(),
                Value = stat.AverageValue,
            })
            .DistinctBy(x => x.Id)
            .ToList();

        if (stats.Count == 0) return ninjaDefinition.Stash!.Stats == null;
        if (ninjaDefinition.Stash!.Stats?.Count != stats.Count) return false;

        foreach (var expectedStat in ninjaDefinition.Stash.Stats)
        {
            var foundStat = stats.FirstOrDefault(stat => stat.Id == expectedStat.Id);
            if (foundStat == null) return false;

            if (expectedStat.Value != null && expectedStat.Value != 0 &&
                (int)Math.Round(foundStat.Value) != expectedStat.Value) return false;
        }

        return true;
    }

    private async Task<List<NinjaStash>> BuildResult(List<NinjaItemDefinition> items)
    {
        items = items.Where(x => x.Stash != null).ToList();
        var variants = items.DistinctBy(x => x.Type);
        if (items.Count == 0 || variants.Count() > 1) return [];

        var result = await GetResult(items.First().Type);
        if (result == null) return [];

        return await GetNinjaStashes();

        async Task<List<NinjaStash>> GetNinjaStashes()
        {
            var results = new List<NinjaStash>();
            foreach (var item in items)
            {
                if (item.Stash == null) continue;
                var line = result.Lines.FirstOrDefault(x => x.DetailsId == item.Stash.DetailsId);
                if (line == null) continue;

                results.Add(new NinjaStash(line, result)
                {
                    DetailsUrl = await ninjaUriProvider.GetDetailsUri(item),
                    Definition = item,
                });
            }

            return results
                .OrderBy(x => x.ChaosValue)
                .ToList();
        }
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
