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

    public List<NinjaStashDefinition> GetDefinitions(Item item)
    {
        if (item.InvariantTradeItem == null) return [];

        if (item.Properties.Rarity == Rarity.Unique)
        {
            return GetUniqueInfo(item.InvariantTradeItem,
                                 item.Properties.Foulborn,
                                 item.Properties.GetMaximumNumberOfLinks(),
                                 item.Stats);
        }

        if (item.Properties.MapTier > 0 || item.ItemClass.Type == ItemClass.Map)
        {
            return GetMapInfo(item.InvariantTradeItem,
                              item.Properties.MapTier,
                              item.Stats);
        }

        if (item.Properties.GemLevel > 0)
        {
            return GetGemInfo(item.InvariantTradeItem,
                              item.Properties.Corrupted,
                              item.Properties.GemLevel,
                              item.Properties.Quality);
        }

        if (IsClusterJewel(item.InvariantTradeItem))
        {
            return GetClusterJewelInfo(item.InvariantTradeItem,
                                       item.Properties.ItemLevel,
                                       item.Stats);

        }

        if (item.InvariantTradeItem.Category == "monster")
        {
            return GetBeastInfo(item.InvariantTradeItem);
        }

        return GetBaseTypeInfo(item.InvariantTradeItem,
                               item.Properties.ItemLevel,
                               item.Properties.Influences);
    }

    public async Task<List<NinjaStash>> GetInfo(Item item)
    {
        var definitions = GetDefinitions(item);
        return await BuildResult(definitions);
    }

    public List<NinjaStashDefinition> GetDefinitions(TradeItemDefinition item, ApiItem apiItem)
    {
        var stats = apiItem.ExplicitMods
            .Where(x => x.Flags?.Mutated ?? false)
            .Select(x => statParser.ParseInvariant(StatCategory.Mutated, x.Description)!).ToList();
        stats.AddRange(apiItem.EnchantMods.Select(x => statParser.ParseInvariant(StatCategory.Enchant, x.Description)!).ToList());
        stats.AddRange(apiItem.ImplicitMods.Select(x => statParser.ParseInvariant(StatCategory.Implicit, x.Description)!).ToList());
        stats = stats.Where(x => x != null!).ToList();

        if (apiItem.Rarity == Rarity.Unique)
        {
            return GetUniqueInfo(item,
                                 apiItem.Mutated,
                                 apiItem.MaxLinks,
                                 stats);
        }

        if (apiItem.GemLevel > 0)
        {
            return GetGemInfo(item,
                              apiItem.Corrupted,
                              apiItem.GemLevel.Value,
                              apiItem.Quality.GetValueOrDefault());
        }

        if (apiItem.MapTier > 0 || item.Category == "map")
        {
            return GetMapInfo(item,
                              apiItem.MapTier,
                              stats);
        }

        if (IsClusterJewel(item))
        {
            return GetClusterJewelInfo(item,
                                       apiItem.ItemLevel,
                                       stats);

        }

        if (item.Category == "monster")
        {
            return GetBeastInfo(item);
        }

        return GetBaseTypeInfo(item,
                               apiItem.ItemLevel,
                               apiItem.Influences);
    }

    public async Task<List<NinjaStash>> GetInfo(TradeItemDefinition item, ApiItem apiItem)
    {
        var definitions = GetDefinitions(item, apiItem);
        return await BuildResult(definitions);
    }

    private List<NinjaStashDefinition> GetUniqueInfo(TradeItemDefinition item, bool foulborn, int? links, List<Stat>? stats)
    {
        if (item.NinjaItems == null) return [];

        if (links < 5) links = 0;

        return item.NinjaItems
            .Where(x => x.Foulborn == foulborn)
            .Where(x => x.Links.GetValueOrDefault() == links.GetValueOrDefault())
            .Where(x => ValidateNinjaStats(stats, StatCategory.Mutated, x))
            .ToList();
    }

    private List<NinjaStashDefinition> GetMapInfo(TradeItemDefinition item, int? mapTier, List<Stat>? stats)
    {
        if (item.NinjaItems == null) return [];
        if (string.IsNullOrEmpty(item.Type)) return [];

        if (stats != null && stats.Any(x => x.Category == StatCategory.Implicit))
        {
            var statsResults = item.NinjaItems
                .Where(x => ValidateNinjaStats(stats, StatCategory.Implicit, x))
                .ToList();
            if (statsResults.Count > 0) return statsResults;
        }

        var type = item.Type;
        if (mapTier.HasValue)
        {
            if (type == "Map") type = $"Map (Tier {mapTier})";
            if (type == "Blighted Map") type = $"Blighted Map (Tier {mapTier})";
            if (type == "Blight-ravaged Map") type = $"Blight-ravaged Map (Tier {mapTier})";
        }

        return item.NinjaItems.ToList();
    }

    private List<NinjaStashDefinition> GetGemInfo(TradeItemDefinition item, bool corrupted, int gemLevel, int gemQuality)
    {
        if (item.NinjaItems == null) return [];
        if (string.IsNullOrEmpty(item.Name)) return [];

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
            .Where(x => x.GemLevel.GetValueOrDefault() == gemLevel)
            .Where(x => x.GemQuality.GetValueOrDefault() == gemQuality)
            .Where(x => x.Corrupted == corrupted)
            .ToList();
    }

    private bool IsClusterJewel(TradeItemDefinition item)
    {
        if (item.NinjaItems == null) return false;
        return item.Name is "Small Cluster Jewel" or "Medium Cluster Jewel" or "Large Cluster Jewel";
    }

    private List<NinjaStashDefinition> GetClusterJewelInfo(TradeItemDefinition item, int itemLevel, List<Stat>? stats)
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
            .Where(x => x.ItemLevel.GetValueOrDefault() == itemLevel)
            .Where(x => x.DetailsId == "6-increased-mana-reservation-efficiency-of-skills-3-passives-75")
            .Where(x => ValidateNinjaStats(stats, StatCategory.Enchant, x))
            .ToList();
    }

    private List<NinjaStashDefinition> GetBaseTypeInfo(TradeItemDefinition item, int itemLevel, Influences influences)
    {
        if (item.NinjaItems == null) return [];
        if (string.IsNullOrEmpty(item.Name)) return [];

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
            .Where(x => x.ItemLevel.GetValueOrDefault() == itemLevel)
            .Where(x => (x.Variant == null && variants.Count == 0) || (x.Variant != null && variants.Contains(x.Variant)))
            .ToList();

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

    private List<NinjaStashDefinition> GetBeastInfo(TradeItemDefinition item)
    {
        if (item.NinjaItems == null) return [];
        if (string.IsNullOrEmpty(item.Name)) return [];

        return item.NinjaItems.ToList();
    }

    private static bool ValidateNinjaStats(List<Stat>? itemStats, StatCategory statCategory, NinjaStashDefinition ninjaDefinition)
    {
        var statStartsWith = statCategory.GetValueAttribute();
        if (statCategory == StatCategory.Mutated) statStartsWith = "explicit";

        var stats = (
            from stat in itemStats
            from definition in stat.Definitions
            where definition?.TradeIds != null
            where !IgnoreStatTexts.Contains(definition.Text)
            from tradeStatId in definition.TradeIds!
            where stat.Category == statCategory && tradeStatId.StartsWith(statStartsWith)
            select new
            {
                Id = tradeStatId,
                stat.Values,
            })
            .Distinct()
            .ToList();

        if (stats.Count == 0 && (ninjaDefinition.Stats?.Count ?? 0) == 0) return true;
        if (ninjaDefinition.Stats?.Count != stats.Count) return false;

        foreach (var expectedStat in ninjaDefinition.Stats)
        {
            var foundStat = stats.FirstOrDefault(stat => stat.Id == expectedStat.Id);
            foundStat ??= stats.FirstOrDefault(stat => stat.Id == expectedStat.Id?.Replace('#', '|'));
            if (foundStat == null) return false;

            if (expectedStat.Min == null || expectedStat.Max == null || expectedStat.Min == 0 || expectedStat.Max == 0) continue;
            var min = foundStat.Values.Count > 0 ? foundStat.Values.Min() : 0;
            var max = foundStat.Values.Count > 0 ? foundStat.Values.Max() : 0;
            if (min < expectedStat.Min || max > expectedStat.Max) return false;
        }

        return true;
    }

    private async Task<List<NinjaStash>> BuildResult(List<NinjaStashDefinition> items)
    {
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
                var line = result.Lines.FirstOrDefault(x => x.DetailsId == item.DetailsId);
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
