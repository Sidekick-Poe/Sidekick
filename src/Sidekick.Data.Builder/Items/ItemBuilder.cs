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
    IGameLanguageProvider languageProvider,
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
        var itemsResult = await dataProvider.Read<RawTradeResult<List<RawTradeItemCategory>>>(game, DataType.RawTradeItems, language);
        // TODO remove
        var invariantItems = await GetInvariantDictionary(game, language);
        var repoeBaseItems = await GetBaseItems(game, language);

        var staticItems = await GetStaticDictionary(game, language);
        StaticItem? GetStatic(string? name, string? type)
        {
            var data = !string.IsNullOrEmpty(name) ? staticItems.FirstOrDefault(x => x.Text == name) : null;
            data ??= !string.IsNullOrEmpty(type) ? staticItems.FirstOrDefault(x => x.Text == type) : null;
            return data;
        }

        var list = new List<ItemDefinition>();
        foreach (var category in itemsResult.Result)
        {
            foreach (var entry in category.Entries)
            {
                var staticItem = GetStatic(entry.Name, entry.Type);
                var text = staticItem?.Text ?? entry.Text;
                if (text == entry.Name || text == entry.Type) text = null;

                var item = new ItemDefinition
                {
                    Id = staticItem?.Id,
                    Image = staticItem?.Image,
                    Name = entry.Name,
                    Type = entry.Type,
                    Text = text,
                    Category = category.Id,
                    IsUnique = entry.IsUnique,
                    Discriminator = entry.Discriminator,
                    BaseItem = repoeBaseItems.GetValueOrDefault(entry.Type),
                };

                FillInvariant(item);
                FillPatterns(item);

                list.Add(item);
            }
        }

        await dataProvider.Write(game, DataType.Items, language, list);

        return;

        void FillInvariant(ItemDefinition item)
        {
            if (language.Code == languageProvider.InvariantLanguage.Code)
            {
                item.InvariantText = item.Text;
                item.InvariantName = item.Name;
                item.InvariantType = item.Type;
                return;
            }

            if (invariantItems == null) return;

            var invariant = invariantItems.GetValueOrDefault(item.Id ?? string.Empty);
            if (invariant != null)
            {
                item.InvariantText = invariant.Text;
                item.InvariantName = invariant.Name;
                item.InvariantType = invariant.Type;
            }
        }

        void FillPatterns(ItemDefinition item)
        {
            if (!string.IsNullOrEmpty(item.Name))
            {
                var regex = $"^{Regex.Escape(item.Name)}|{Regex.Escape(item.Name)}$";
                item.NamePattern = new Regex(regex);
            }

            if (!string.IsNullOrEmpty(item.Text))
            {
                item.TextPattern = new Regex(Regex.Escape(item.Text));
            }

            if (!item.IsUnique && !string.IsNullOrEmpty(item.Type))
            {
                var regex = $@"(?<!\p{{L}}){Regex.Escape(item.Type)}(?!\p{{L}})";
                item.TypePattern = new Regex(regex);
            }
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

    private async Task<Dictionary<string, ItemDefinition>?> GetInvariantDictionary(GameType game, IGameLanguage language)
    {
        if (language.Code == languageProvider.InvariantLanguage.Code) return null;

        var raw = await dataProvider.Read<List<ItemDefinition>>(game, DataType.Items, languageProvider.InvariantLanguage);
        return raw.Where(x => x.Id != null)
            .DistinctBy(x => x.Id)
            .ToDictionary(x => x.Id!, x => x);
    }

    private async Task<Dictionary<string, ItemClassDefinition2>> GetItemClassDefinitions(GameType game, IGameLanguage language)
    {
        var raw = await repoeDownloader.ReadItemClasses(game, language.Code);
        var result = new Dictionary<string, ItemClassDefinition2>();

        foreach (var entry in raw)
        {
            var definition = new ItemClassDefinition2()
            {
                Id = entry.Key,
                Name = entry.Value.Name,
                Type = entry.Key.GetEnumFromValue<ItemClass>(),
            };

            result.Add(definition.Id, definition);
        }

        return result;
    }

    private async Task<Dictionary<string, BaseItemDefinition>> GetBaseItems(GameType game, IGameLanguage language)
    {
        var repoeBaseItems = await repoeDownloader.ReadBaseItems(game, language.Code);
        var repoeItemClasses = await GetItemClassDefinitions(game, language);

        var result = new Dictionary<string, BaseItemDefinition>();
        foreach (var group in repoeBaseItems.GroupBy(x => x.Value.Name))
        {
            var repoeBaseItem = group.First();
            if (group.Count() > 1)
            {
                logger.LogWarning("Multiple matches found for '{0}' in game '{1}'.", group.Key, game.GetValueAttribute());
                repoeBaseItem = group
                    .OrderBy(x => x.Key.Contains("Royale") ? 1 : 0)
                    .ThenBy(x => x.Key.Length)
                    .FirstOrDefault();
            }

            if (string.IsNullOrEmpty(repoeBaseItem.Value.Name)) continue;

            result.Add(repoeBaseItem.Value.Name, new BaseItemDefinition()
            {
                Id = repoeBaseItem.Key,
                ItemClass = repoeBaseItem.Value.ItemClass != null ? repoeItemClasses.GetValueOrDefault(repoeBaseItem.Value.ItemClass) : null,
                Name = repoeBaseItem.Value.Name,
                Requirements = repoeBaseItem.Value.Requirements != null ? new ItemRequirements()
                {
                    Dexterity = repoeBaseItem.Value.Requirements.Dexterity,
                    Strength = repoeBaseItem.Value.Requirements.Strength,
                    Intelligence = repoeBaseItem.Value.Requirements.Intelligence,
                    Level = repoeBaseItem.Value.Requirements.Level,
                } : null,
                Properties = repoeBaseItem.Value.Properties != null ? new ItemProperties()
                {
                    Armour = GetPropertyValues(repoeBaseItem.Value.Properties.Armour),
                    EnergyShield = GetPropertyValues(repoeBaseItem.Value.Properties.EnergyShield),
                    Evasion = GetPropertyValues(repoeBaseItem.Value.Properties.Evasion),
                    PhysicalDamage = repoeBaseItem.Value.Properties.PhysicalDamageMin != null ? new ItemPropertyValues()
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

        ItemPropertyValues? GetPropertyValues(RepoeBaseItemProperty? property)
        {
            if (property == null) return null;
            return new ItemPropertyValues()
            {
                Max = property.Max,
                Min = property.Min,
            };
        }
    }
}
