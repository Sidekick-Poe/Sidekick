using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Common.Enums;
using Sidekick.Data.Builder.Ninja;
using Sidekick.Data.Builder.Repoe;
using Sidekick.Data.Builder.Repoe.Models.Items;
using Sidekick.Data.Builder.Trade.Models;
using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Languages;
namespace Sidekick.Data.Builder.ItemDefinitions;

public class ItemDefinitionBuilder(
    ILogger<ItemDefinitionBuilder> logger,
    IOptions<SidekickConfiguration> configuration,
    DataProvider dataProvider,
    IGameLanguageProvider gameLanguageProvider,
    RepoeDownloader repoeDownloader,
    NinjaDownloader ninjaDownloader)
{
    private static readonly Dictionary<string, string> BaseItemToTradeItemMappings = new()
    {
        { "Metadata/Items/Maps/MapKeyTier1", "Metadata/Items/TradeProxy/MapKey" }, // (PoE1) Maps are not on the trade, we map them to the main Map item.
        { "Metadata/Items/Maps/MapKeyTier2", "Metadata/Items/TradeProxy/MapKey" }, // (PoE1) Maps are not on the trade, we map them to the main Map item.
        { "Metadata/Items/Maps/MapKeyTier3", "Metadata/Items/TradeProxy/MapKey" }, // (PoE1) Maps are not on the trade, we map them to the main Map item.
        { "Metadata/Items/Maps/MapKeyTier4", "Metadata/Items/TradeProxy/MapKey" }, // (PoE1) Maps are not on the trade, we map them to the main Map item.
        { "Metadata/Items/Maps/MapKeyTier5", "Metadata/Items/TradeProxy/MapKey" }, // (PoE1) Maps are not on the trade, we map them to the main Map item.
        { "Metadata/Items/Maps/MapKeyTier6", "Metadata/Items/TradeProxy/MapKey" }, // (PoE1) Maps are not on the trade, we map them to the main Map item.
        { "Metadata/Items/Maps/MapKeyTier7", "Metadata/Items/TradeProxy/MapKey" }, // (PoE1) Maps are not on the trade, we map them to the main Map item.
        { "Metadata/Items/Maps/MapKeyTier8", "Metadata/Items/TradeProxy/MapKey" }, // (PoE1) Maps are not on the trade, we map them to the main Map item.
        { "Metadata/Items/Maps/MapKeyTier9", "Metadata/Items/TradeProxy/MapKey" }, // (PoE1) Maps are not on the trade, we map them to the main Map item.
        { "Metadata/Items/Maps/MapKeyTier10", "Metadata/Items/TradeProxy/MapKey" }, // (PoE1) Maps are not on the trade, we map them to the main Map item.
        { "Metadata/Items/Maps/MapKeyTier11", "Metadata/Items/TradeProxy/MapKey" }, // (PoE1) Maps are not on the trade, we map them to the main Map item.
        { "Metadata/Items/Maps/MapKeyTier12", "Metadata/Items/TradeProxy/MapKey" }, // (PoE1) Maps are not on the trade, we map them to the main Map item.
        { "Metadata/Items/Maps/MapKeyTier13", "Metadata/Items/TradeProxy/MapKey" }, // (PoE1) Maps are not on the trade, we map them to the main Map item.
        { "Metadata/Items/Maps/MapKeyTier14", "Metadata/Items/TradeProxy/MapKey" }, // (PoE1) Maps are not on the trade, we map them to the main Map item.
        { "Metadata/Items/Maps/MapKeyTier15", "Metadata/Items/TradeProxy/MapKey" }, // (PoE1) Maps are not on the trade, we map them to the main Map item.
        { "Metadata/Items/Maps/MapKeyTier16", "Metadata/Items/TradeProxy/MapKey" }, // (PoE1) Maps are not on the trade, we map them to the main Map item.
        { "Metadata/Items/TradeProxy/BlightedMap", "Metadata/Items/TradeProxy/MapKey" }, // (PoE1) Blighted maps are not on the trade, we map them to the main Map item.
        { "Metadata/Items/TradeProxy/UberBlightedMap", "Metadata/Items/TradeProxy/MapKey" }, // (PoE1) Blight-ravaged maps are not on the trade, we map them to the main Map item.
    };

    public async Task Build(IGameLanguage language)
    {
        try
        {
            await BuildForGame(GameType.PathOfExile1, language);
            await BuildForGame(GameType.PathOfExile2, language);
        }
        catch (Exception ex)
        {
            if (configuration.Value.ApplicationType == SidekickApplicationType.DataBuilder || configuration.Value.ApplicationType == SidekickApplicationType.Test)
            {
                throw;
            }

            logger.LogError(ex, "Failed to build items data.");
        }
    }

    private async Task BuildForGame(GameType game, IGameLanguage language)
    {
        var tradeItems = await GetTradeItems(game, language);
        var baseItems = await GetBaseItems(game, language);
        var uniqueItems = await GetUniqueItems(game, language);
        var ninjaItems = await ninjaDownloader.GetDefinitions(game);

        var list = new List<ItemDefinition>();
        foreach (var tradeItem in tradeItems)
        {
            var baseItem = baseItems.FirstOrDefault(x => !string.IsNullOrEmpty(tradeItem.Text) && x.Name == tradeItem.Text);
            baseItem ??= baseItems.FirstOrDefault(x => x.Name == tradeItem.Type);

            var uniqueItem = uniqueItems.FirstOrDefault(x => x.Name == tradeItem.Name);

            list.Add(new ItemDefinition
            {
                TradeItem = tradeItem,
                BaseItem = baseItem,
                UniqueItem = uniqueItem,
                NinjaItems = GetNinjaItems(tradeItem, baseItem, uniqueItem),
                NamePattern = GetNamePattern(tradeItem),
                TypePattern = GetTypePattern(tradeItem.Type, uniqueItem != null),
                TextPattern = GetTextPattern(tradeItem, uniqueItem != null),
            });
        }

        foreach (var baseItem in baseItems)
        {
            if (list.Any(x => x.BaseItem == baseItem)) continue;

            var tradeItem = tradeItems.FirstOrDefault(x => x.Type == baseItem.Name);
            tradeItem ??= tradeItems.FirstOrDefault(x => x.Name == baseItem.Name);
            tradeItem ??= tradeItems.FirstOrDefault(x => x.Text == baseItem.Name);

            if (tradeItem == null && baseItem.Id != null && BaseItemToTradeItemMappings.TryGetValue(baseItem.Id, out var mapping))
            {
                var baseItemMapping = baseItems.FirstOrDefault(x => x.Id == mapping);
                if (baseItemMapping != null)
                {
                    tradeItem ??= tradeItems.FirstOrDefault(x => x.Type == baseItemMapping.Name);
                    tradeItem ??= tradeItems.FirstOrDefault(x => x.Name == baseItemMapping.Name);
                    tradeItem ??= tradeItems.FirstOrDefault(x => x.Text == baseItemMapping.Name);
                }
            }

            list.Add(new ItemDefinition
            {
                TradeItem = tradeItem,
                BaseItem = baseItem,
                NinjaItems = GetNinjaItems(tradeItem, baseItem, null),
                TypePattern = tradeItem == null ? null : GetTypePattern(baseItem.Name, false), // Do not create a pattern if tradeItem is null.
            });
        }

        await dataProvider.Write(game, DataType.Items, language, list);

        return;

        Regex? GetNamePattern(TradeItemDefinition? tradeItem)
        {
            if (string.IsNullOrEmpty(tradeItem?.Name)) return null;

            var regex = $"^{Regex.Escape(tradeItem.Name)}|{Regex.Escape(tradeItem.Name)}$";
            return new Regex(regex);
        }

        Regex? GetTypePattern(string? type, bool isUnique)
        {
            if (string.IsNullOrEmpty(type) || isUnique) return null;

            var regex = $@"(?<!\p{{L}}){Regex.Escape(type)}(?!\p{{L}})";
            return new Regex(regex);
        }

        Regex? GetTextPattern(TradeItemDefinition? tradeItem, bool isUnique)
        {
            if (string.IsNullOrEmpty(tradeItem?.Text) || tradeItem.Text == tradeItem.Type || isUnique) return null;

            return new Regex(Regex.Escape(tradeItem.Text));
        }

        List<NinjaItemDefinition>? GetNinjaItems(TradeItemDefinition? tradeItem, BaseItemDefinition? baseItem, UniqueItemDefinition? uniqueItem)
        {
            if (language.Code != gameLanguageProvider.InvariantLanguage.Code) return null;

            var result = new List<NinjaItemDefinition>();
            foreach (var ninjaItem in ninjaItems)
            {
                // Exchange items
                if (tradeItem != null &&
                    ninjaItem.Exchange?.Id != null &&
                    ninjaItem.Exchange.Id == tradeItem.Id) result.Add(ninjaItem);

                // Unique items, support for foulborn uniques
                else if (uniqueItem != null)
                {
                    if (ninjaItem.Stash?.Name != null &&
                        (
                        ninjaItem.Stash.Name == uniqueItem.Name ||
                        ninjaItem.Stash.Name == $"Foulborn {uniqueItem.Name}"
                        )) result.Add(ninjaItem);
                }

                // Text comparison, support for transfigured gems
                else if (tradeItem is
                         {
                             Text: not null,
                             Name: null,
                         })
                {
                    if (ninjaItem.Stash?.Name != null &&
                        ninjaItem.Stash.Name == tradeItem.Text) result.Add(ninjaItem);
                }

                // Base items, support for maps
                else if (baseItem != null &&
                         ninjaItem.Stash?.Name != null &&
                         (
                         ninjaItem.Stash.Name == baseItem.Name ||
                         (baseItem.Name == "Map (Tier 16)" && ninjaItem.Stash.Name == "Al-Hezmin Map (Tier 16)") ||
                         (baseItem.Name == "Map (Tier 16)" && ninjaItem.Stash.Name == "Baran Map (Tier 16)") ||
                         (baseItem.Name == "Map (Tier 16)" && ninjaItem.Stash.Name == "Drox Map (Tier 16)") ||
                         (baseItem.Name == "Map (Tier 16)" && ninjaItem.Stash.Name == "Veritania Map (Tier 16)") ||
                         (baseItem.Name == "Map (Tier 16)" && ninjaItem.Stash.Name == "The Constrictor Map (Tier 16)") ||
                         (baseItem.Name == "Map (Tier 16)" && ninjaItem.Stash.Name == "The Enslaver Map (Tier 16)") ||
                         (baseItem.Name == "Map (Tier 16)" && ninjaItem.Stash.Name == "The Eradicator Map (Tier 16)") ||
                         (baseItem.Name == "Map (Tier 16)" && ninjaItem.Stash.Name == "The Purifier Map (Tier 16)") ||
                         (baseItem.Name == "Blighted Map" && ninjaItem.Stash.Name.StartsWith("Blighted Map (Tier ")) ||
                         (baseItem.Name == "Blight-ravaged Map" && ninjaItem.Stash.Name == "Blight-ravaged Map (Tier 16)")
                         )) result.Add(ninjaItem);

                // Cluster jewel support
                else if (baseItem != null &&
                         ninjaItem.Stash?.BaseType != null &&
                         ninjaItem.Type != "UniqueJewel" &&
                         ninjaItem.Stash.BaseType == baseItem.Name &&
                         baseItem.Name is "Small Cluster Jewel" or "Medium Cluster Jewel" or "Large Cluster Jewel") result.Add(ninjaItem);

                // Name match
                else if (ninjaItem.Stash?.Name == (baseItem?.Name ?? tradeItem?.Type)) result.Add(ninjaItem);
            }

            return result.Count == 0 ? null : result;
        }
    }

    private record StaticItem(string Id, string? Text, string? Image);

    private async Task<List<StaticItem>> GetStaticDictionary(GameType game, IGameLanguage language)
    {
        var raw = await dataProvider.Read<RawTradeResult<List<RawTradeStaticItemCategory>>>(game, DataType.RawTradeStatic, language);
        var result = new List<StaticItem>();

        foreach (var category in raw.Result)
        {
            foreach (var entry in category.Entries)
            {
                if (entry.Id == null! || entry.Text == null || entry.Id == "sep") continue;

                var image = string.IsNullOrEmpty(entry.Image) ? null : $"https://web.poecdn.com{entry.Image}";
                var item = new StaticItem(entry.Id, entry.Text, image);
                result.Add(item);
            }
        }

        return result;
    }

    private async Task<List<TradeItemDefinition>> GetTradeItems(GameType game, IGameLanguage language)
    {
        var itemsResult = await dataProvider.Read<RawTradeResult<List<RawTradeItemCategory>>>(game, DataType.RawTradeItems, language);
        var staticItems = await GetStaticDictionary(game, language);
        StaticItem? GetStatic(string? name, string? type)
        {
            var data = !string.IsNullOrEmpty(name) ? staticItems.FirstOrDefault(x => x.Text == name) : null;
            data ??= !string.IsNullOrEmpty(type) ? staticItems.FirstOrDefault(x => x.Text == type) : null;
            return data;
        }

        var result = new List<TradeItemDefinition>();
        foreach (var category in itemsResult.Result)
        {
            foreach (var entry in category.Entries)
            {
                var staticItem = GetStatic(entry.Name, entry.Type);
                var text = staticItem?.Text ?? entry.Text;
                if (text == entry.Name || text == entry.Type) text = null;

                if (entry.Discriminator == "legacy") continue;

                result.Add(new TradeItemDefinition()
                {
                    Id = staticItem?.Id,
                    Image = staticItem?.Image,
                    Name = entry.Name,
                    Type = entry.Type,
                    Text = text,
                    Category = category.Id,
                    Discriminator = entry.Discriminator,
                });
            }
        }

        return result;
    }

    private async Task<List<BaseItemDefinition>> GetBaseItems(GameType game, IGameLanguage language)
    {
        var repoeBaseItems = await repoeDownloader.ReadBaseItems(game, language.Code);

        var result = new List<BaseItemDefinition>();
        foreach (var group in repoeBaseItems.GroupBy(x => x.Value.Name))
        {
            var repoeBaseItem = group
                .OrderBy(x => x.Key.Contains("Royale") ? 1 : 0)
                .ThenBy(x => x.Key.Length)
                .First();
            if (group.Count() > 1 && !group.ElementAt(1).Key.Contains("Royale"))
            {
                logger.LogDebug("[ItemBuilder] [BaseItems] Multiple matches found for '{0}' in game '{1}'.", group.Key, game.GetValueAttribute());
            }

            if (string.IsNullOrEmpty(repoeBaseItem.Value.Name)) continue;

            result.Add(new BaseItemDefinition()
            {
                Id = repoeBaseItem.Key,
                Name = repoeBaseItem.Value.Name,
                ItemClassId = repoeBaseItem.Value.ItemClass,
                Requirements = repoeBaseItem.Value.Requirements != null ? new BaseItemRequirements()
                {
                    Dexterity = repoeBaseItem.Value.Requirements.Dexterity,
                    Strength = repoeBaseItem.Value.Requirements.Strength,
                    Intelligence = repoeBaseItem.Value.Requirements.Intelligence,
                    Level = repoeBaseItem.Value.Requirements.Level,
                } : null,
                Properties = repoeBaseItem.Value.Properties is { HasValues: true } ? new BaseItemProperties()
                {
                    Armour = GetPropertyValues(repoeBaseItem.Value.Properties.Armour),
                    EnergyShield = GetPropertyValues(repoeBaseItem.Value.Properties.EnergyShield),
                    Evasion = GetPropertyValues(repoeBaseItem.Value.Properties.Evasion),
                    Ward = GetPropertyValues(repoeBaseItem.Value.Properties.Ward),
                    PhysicalDamage = repoeBaseItem.Value.Properties.PhysicalDamageMin != null ? new BaseItemPropertyValues()
                    {
                        Min = repoeBaseItem.Value.Properties.PhysicalDamageMin,
                        Max = repoeBaseItem.Value.Properties.PhysicalDamageMax,
                    } : null,
                    Block = repoeBaseItem.Value.Properties.Block,
                    AttacksPerSecond = repoeBaseItem.Value.Properties.AttackMilliseconds.HasValue ? (repoeBaseItem.Value.Properties.AttackMilliseconds / 1000d) : null,
                    CriticalHitChance = repoeBaseItem.Value.Properties.CriticalHitChance,
                } : null,
            });
        }

        return result;

        BaseItemPropertyValues? GetPropertyValues(RepoeBaseItemProperty? property)
        {
            if (property == null) return null;
            return new BaseItemPropertyValues()
            {
                Max = property.Max,
                Min = property.Min,
            };
        }
    }

    private async Task<List<UniqueItemDefinition>> GetUniqueItems(GameType game, IGameLanguage language)
    {
        var repoeUniqueItems = await repoeDownloader.ReadUniques(game, language.Code);

        var result = new List<UniqueItemDefinition>();
        foreach (var group in repoeUniqueItems.GroupBy(x => x.Value.Name))
        {
            var repoeUniqueItem = group.First();
            if (string.IsNullOrEmpty(repoeUniqueItem.Value.Name)) continue;

            if (group.Count() > 1)
            {
                logger.LogDebug("[ItemBuilder] [Uniques] Multiple matches found for '{0}' in game '{1}'. Adding first entry.", repoeUniqueItem.Value.Name, game.GetValueAttribute());
            }

            string? image = null;
            if (!string.IsNullOrEmpty(repoeUniqueItem.Value.Visual?.File))
            {
                var gameSlug = game == GameType.PathOfExile1 ? string.Empty : "poe2/";
                image = $"https://repoe-fork.github.io/{gameSlug}{repoeUniqueItem.Value.Visual.File.Replace(".dds", ".png")}";
            }

            result.Add(new UniqueItemDefinition()
            {
                Id = repoeUniqueItem.Value.Id,
                ItemClassId = repoeUniqueItem.Value.ItemClass,
                Name = repoeUniqueItem.Value.Name,
                Image = image,
            });
        }

        return result;
    }
}
