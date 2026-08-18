using Sidekick.Apis.Poe.Trade.Trade.Models;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Stash.Models;
using Sidekick.Apis.PoeNinja.Uris;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;
using Sidekick.Game;
using Sidekick.Game.ItemDefinitions;
using Sidekick.Game.Parser;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Stats;

namespace Sidekick.Apis.PoeNinja.Stash;

public class NinjaStashProvider(
    INinjaClient ninjaClient,
    ISettingsService settingsService,
    ICacheProvider cacheProvider,
    NinjaUriProvider ninjaUriProvider,
    StatParser statParser) : INinjaStashProvider
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

    public List<NinjaStashItem> GetDefinitions(Item item)
    {
        if (item.InvariantDefinition == null) return [];

        if (item.Properties.Rarity == Rarity.Unique)
        {
            return FilterUniqueItems(item.InvariantDefinition.NinjaItems,
                                     item.Properties.Foulborn,
                                     item.Properties.GetMaximumNumberOfLinks(),
                                     item.Stats);
        }

        if (item.Properties.MapTier > 0 || IsMap(item.ItemClass.Id))
        {
            return FilterMapItems(item.InvariantDefinition.NinjaItems,
                                  item.InvariantDefinition.Name,
                                  item.Properties.MapTier,
                                  item.Stats);
        }

        if (item.Properties.GemLevel > 0)
        {
            return FilterGems(item.InvariantDefinition.NinjaItems,
                              item.Properties.Corrupted,
                              item.Properties.GemLevel,
                              item.Properties.Quality);
        }

        if (IsClusterJewel(item.InvariantDefinition.Name))
        {
            return FilterClusterJewels(item.InvariantDefinition.NinjaItems,
                                       item.Properties.ItemLevel,
                                       item.Stats);

        }

        if (item.InvariantDefinition.TradeItems?.FirstOrDefault()?.Category == "monster")
        {
            return FilterBeastiaryMonsters(item.InvariantDefinition.NinjaItems);
        }

        return FilterBaseTypes(item.InvariantDefinition.NinjaItems,
                               item.Properties.ItemLevel,
                               item.Properties.Influences);
    }

    public async Task<List<NinjaStash>> GetInfo(Item item)
    {
        var definitions = GetDefinitions(item);
        return await BuildResult(definitions);
    }

    public List<NinjaStashItem> GetDefinitions(GameType game, ItemDefinition item, ApiItem apiItem)
    {
        var stats = apiItem.ExplicitMods
            .Where(x => x.Flags?.Mutated ?? false)
            .Select(x => statParser.ParseInvariant(StatCategory.Mutated, x.Description)!).ToList();
        stats.AddRange(apiItem.EnchantMods.Select(x => statParser.ParseInvariant(StatCategory.Enchant, x.Description)!).ToList());
        stats.AddRange(apiItem.ImplicitMods.Select(x => statParser.ParseInvariant(StatCategory.Implicit, x.Description)!).ToList());
        stats = stats.Where(x => x != null!).ToList();

        if (apiItem.Rarity == Rarity.Unique)
        {
            return FilterUniqueItems(item.NinjaItems,
                                     apiItem.Mutated,
                                     apiItem.MaxLinks,
                                     stats);
        }

        if (apiItem.GemLevel > 0)
        {
            return FilterGems(item.NinjaItems,
                              apiItem.Corrupted,
                              apiItem.GemLevel.Value,
                              apiItem.Quality.GetValueOrDefault());
        }

        if (IsClusterJewel(item.Name))
        {
            return FilterClusterJewels(item.NinjaItems,
                                       apiItem.ItemLevel,
                                       stats);

        }

        if (apiItem.MapTier > 0 || item.BaseItems.Any(x => IsMap(x.ItemClassId)))
        {
            return FilterMapItems(item.NinjaItems,
                                  item.Name,
                                  apiItem.MapTier,
                                  stats);
        }

        if (item.TradeItems?.FirstOrDefault()?.Category == "monster")
        {
            return FilterBeastiaryMonsters(item.NinjaItems);
        }

        return FilterBaseTypes(item.NinjaItems,
                               apiItem.ItemLevel,
                               apiItem.Influences);
    }

    public async Task<List<NinjaStash>> GetInfo(GameType game, ItemDefinition item, ApiItem apiItem)
    {
        var definitions = GetDefinitions(game, item, apiItem);
        return await BuildResult(definitions);
    }

    private List<NinjaStashItem> FilterUniqueItems(List<NinjaStashItem>? items, bool foulborn, int? links, List<Stat>? stats)
    {
        if (items == null) return [];

        if (links < 5) links = 0;

        var query = items
            .Where(x => x.Foulborn == foulborn)
            .Where(x => x.Links.GetValueOrDefault() == links.GetValueOrDefault());

        query = ValidateNinjaStats(query, stats, StatCategory.Mutated);
        return query.ToList();
    }

    private bool IsMap(string? itemClassId)
    {
        return itemClassId == "Map" || itemClassId == "MapKey" || itemClassId == "InstanceLocalItem";
    }

    private List<NinjaStashItem> FilterMapItems(List<NinjaStashItem>? items, string? type, int? mapTier, List<Stat>? stats)
    {
        if (items == null) return [];
        if (string.IsNullOrEmpty(type)) return [];

        var query = ValidateNinjaStats(items.AsEnumerable(), stats, StatCategory.Implicit);

        if (mapTier.HasValue)
        {
            query = query.Where(x => x.MapTier == null || x.MapTier == mapTier);
        }

        return query.ToList();
    }

    private List<NinjaStashItem> FilterGems(List<NinjaStashItem>? items, bool corrupted, int gemLevel, int gemQuality)
    {
        if (items == null) return [];

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

        return items
            .Where(x => x.GemLevel.GetValueOrDefault() == gemLevel)
            .Where(x => x.GemQuality.GetValueOrDefault() == gemQuality)
            .Where(x => x.Corrupted == corrupted)
            .ToList();
    }

    private bool IsClusterJewel(string? name)
    {
        return name is "Small Cluster Jewel" or "Medium Cluster Jewel" or "Large Cluster Jewel";
    }

    private List<NinjaStashItem> FilterClusterJewels(List<NinjaStashItem>? items, int itemLevel, List<Stat>? stats)
    {
        if (items == null) return [];

        itemLevel = itemLevel switch
        {
            < 50 => 1,
            < 68 => 50,
            < 75 => 68,
            < 84 => 75,
            _ => 84,
        };

        var query = items.Where(x => x.ItemLevel.GetValueOrDefault() == itemLevel);
        query = ValidateNinjaStats(query, stats, StatCategory.Enchant);
        return query.ToList();
    }

    private List<NinjaStashItem> FilterBaseTypes(List<NinjaStashItem>? items, int itemLevel, Influences influences)
    {
        if (items == null) return [];

        var variants = GetVariants().ToList();
        var query = items.AsQueryable();

        if (items.Any(x => x.ItemLevel != null))
        {
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
            query = query.Where(x => x.ItemLevel.GetValueOrDefault() == itemLevel);
        }

        return query
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

    private List<NinjaStashItem> FilterBeastiaryMonsters(List<NinjaStashItem>? items)
    {
        return items ?? [];
    }

    private static IEnumerable<NinjaStashItem> ValidateNinjaStats(IEnumerable<NinjaStashItem> query, List<Stat>? itemStats, StatCategory statCategory)
    {
        var outputEmpty = true;
        var items = query.ToList();
        foreach (var ninjaItem in items)
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

            if (ninjaItem.Stats?.Count != stats.Count)
            {
                continue;
            }

            var valid = true;
            foreach (var expectedStat in ninjaItem.Stats)
            {
                var foundStat = stats.FirstOrDefault(stat => stat.Id == expectedStat.Id);
                foundStat ??= stats.FirstOrDefault(stat => stat.Id == expectedStat.Id?.Replace('#', '|'));
                if (foundStat == null)
                {
                    valid = false;
                    break;
                }

                if (expectedStat.Min == null || expectedStat.Max == null || expectedStat.Min == 0 || expectedStat.Max == 0)
                {
                    continue;
                }
                var min = foundStat.Values.Count > 0 ? foundStat.Values.Min() : 0;
                var max = foundStat.Values.Count > 0 ? foundStat.Values.Max() : 0;
                if (min < expectedStat.Min || max > expectedStat.Max)
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                outputEmpty = false;
                yield return ninjaItem;
            }
        }

        if (!outputEmpty) yield break;

        foreach (var ninjaItem in items)
        {
            if ((ninjaItem.Stats?.Count ?? 0) == 0)
            {
                yield return ninjaItem;
            }
        }
    }

    private async Task<List<NinjaStash>> BuildResult(List<NinjaStashItem> items)
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
                    Item = item,
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
