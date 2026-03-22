using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Common.Enums;
using Sidekick.Data.Builder.Repoe;
using Sidekick.Data.Builder.Repoe.Models.Items;
using Sidekick.Data.Builder.Trade.Models;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
namespace Sidekick.Data.Builder.Items;

public class ItemBuilder(
    ILogger<ItemBuilder> logger,
    IOptions<SidekickConfiguration> configuration,
    DataProvider dataProvider,
    RepoeDownloader repoeDownloader)
{
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

        var list = new List<ItemDefinition>();
        foreach (var tradeItem in tradeItems)
        {
            var baseItem = baseItems.FirstOrDefault(x => x.Name == tradeItem.Type);
            baseItem ??= baseItems.FirstOrDefault(x => x.Name == tradeItem.Name);

            var uniqueItem = uniqueItems.FirstOrDefault(x => x.Name == tradeItem.Name);

            list.Add(new ItemDefinition
            {
                TradeItem = tradeItem,
                BaseItem = baseItem,
                UniqueItem = uniqueItem,
                NamePattern = GetNamePattern(tradeItem),
                TypePattern = GetTypePattern(tradeItem),
                TextPattern = GetTextPattern(tradeItem),
            });
        }

        foreach (var baseItem in baseItems)
        {
            if (list.Any(x => x.BaseItem == baseItem)) continue;

            var tradeItem = tradeItems.FirstOrDefault(x => x.Type == baseItem.Name);
            tradeItem ??= tradeItems.FirstOrDefault(x => x.Name == baseItem.Name);
            tradeItem ??= tradeItems.FirstOrDefault(x => x.Text == baseItem.Name);

            list.Add(new ItemDefinition
            {
                TradeItem = tradeItem,
                BaseItem = baseItem,
                NamePattern = GetNamePattern(tradeItem),
                TypePattern = GetTypePattern(tradeItem),
                TextPattern = GetTextPattern(tradeItem),
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

        Regex? GetTypePattern(TradeItemDefinition? tradeItem)
        {
            if (string.IsNullOrEmpty(tradeItem?.Type) || tradeItem.IsUnique) return null;

            var regex = $@"(?<!\p{{L}}){Regex.Escape(tradeItem.Type)}(?!\p{{L}})";
            return new Regex(regex);
        }

        Regex? GetTextPattern(TradeItemDefinition? tradeItem)
        {
            if (string.IsNullOrEmpty(tradeItem?.Text)) return null;

            return new Regex(Regex.Escape(tradeItem.Text));
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

                result.Add(new TradeItemDefinition()
                {
                    Id = staticItem?.Id,
                    Image = staticItem?.Image,
                    Name = entry.Name,
                    Type = entry.Type,
                    Text = text,
                    Category = category.Id,
                    IsUnique = entry.IsUnique,
                    Discriminator = entry.Discriminator,
                });
            }
        }

        return result;
    }

    private async Task<Dictionary<string, ItemClassDefinition>> GetItemClassDefinitions(GameType game, IGameLanguage language)
    {
        var raw = await repoeDownloader.ReadItemClasses(game, language.Code);
        var result = new Dictionary<string, ItemClassDefinition>();

        foreach (var entry in raw)
        {
            var definition = new ItemClassDefinition()
            {
                Id = entry.Key,
                Name = entry.Value.Name,
                Type = entry.Key.GetEnumFromValue<ItemClass>(),
            };

            result.Add(definition.Id, definition);
        }

        return result;
    }

    private async Task<List<BaseItemDefinition>> GetBaseItems(GameType game, IGameLanguage language)
    {
        var repoeBaseItems = await repoeDownloader.ReadBaseItems(game, language.Code);
        var repoeItemClasses = await GetItemClassDefinitions(game, language);

        var result = new List<BaseItemDefinition>();
        foreach (var group in repoeBaseItems.GroupBy(x => x.Value.Name))
        {
            var repoeBaseItem = group
                .OrderBy(x => x.Key.Contains("Royale") ? 1 : 0)
                .ThenBy(x => x.Key.Length)
                .First();
            if (group.Count() > 1 && !group.ElementAt(1).Key.Contains("Royale"))
            {
                logger.LogDebug("Multiple matches found for '{0}' in game '{1}'.", group.Key, game.GetValueAttribute());
            }

            if (string.IsNullOrEmpty(repoeBaseItem.Value.Name)) continue;

            result.Add(new BaseItemDefinition()
            {
                Id = repoeBaseItem.Key,
                ItemClass = repoeBaseItem.Value.ItemClass != null ? repoeItemClasses.GetValueOrDefault(repoeBaseItem.Value.ItemClass) : null,
                Name = repoeBaseItem.Value.Name,
                Requirements = repoeBaseItem.Value.Requirements != null ? new BaseItemRequirements()
                {
                    Dexterity = repoeBaseItem.Value.Requirements.Dexterity,
                    Strength = repoeBaseItem.Value.Requirements.Strength,
                    Intelligence = repoeBaseItem.Value.Requirements.Intelligence,
                    Level = repoeBaseItem.Value.Requirements.Level,
                } : null,
                Properties = repoeBaseItem.Value.Properties != null ? new BaseItemProperties()
                {
                    Armour = GetPropertyValues(repoeBaseItem.Value.Properties.Armour),
                    EnergyShield = GetPropertyValues(repoeBaseItem.Value.Properties.EnergyShield),
                    Evasion = GetPropertyValues(repoeBaseItem.Value.Properties.Evasion),
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
        var repoeItemClasses = await GetItemClassDefinitions(game, language);

        var result = new List<UniqueItemDefinition>();
        foreach (var group in repoeUniqueItems.GroupBy(x => x.Value.Name))
        {
            var repoeUniqueItem = group.First();
            if (group.Count() > 1)
            {
                logger.LogDebug("Multiple matches found for '{0}' in game '{1}'.", group.Key, game.GetValueAttribute());
            }

            if (string.IsNullOrEmpty(repoeUniqueItem.Value.Name)) continue;

            string? image = null;
            if (!string.IsNullOrEmpty(repoeUniqueItem.Value.Visual?.File))
            {
                var gameSlug = game == GameType.PathOfExile1 ? string.Empty : "poe2/";
                image = $"https://repoe-fork.github.io/{gameSlug}{repoeUniqueItem.Value.Visual.File.Replace(".dds", ".png")}";
            }

            result.Add(new UniqueItemDefinition()
            {
                Id = repoeUniqueItem.Value.Id,
                ItemClass = repoeUniqueItem.Value.ItemClass != null ? repoeItemClasses.GetValueOrDefault(repoeUniqueItem.Value.ItemClass) : null,
                Name = repoeUniqueItem.Value.Name,
                Image = image,
            });
        }

        return result;
    }
}
