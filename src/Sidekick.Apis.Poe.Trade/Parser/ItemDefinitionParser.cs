using FuzzySharp;
using Sidekick.Apis.Poe.Trade.Trade.Models;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.ItemClasses;
using Sidekick.Game.ItemDefinitions;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Providers;

namespace Sidekick.Apis.Poe.Trade.Parser;

public class ItemDefinitionParser(
    ICurrentGameLanguage currentGameLanguage,
    ItemClassProvider itemClassProvider,
    BaseItemProvider baseItemProvider,
    ItemDefinitionProvider itemDefinitionProvider
)
{
    public void Parse(Item item)
    {
        item.Type = item.Text.Blocks[0].Lines[^1].Text;
        if (item.Properties.Rarity is Rarity.Rare or Rarity.Unique && item.Text.Blocks[0].Lines.Count >= 4)
        {
            item.Name = item.Text.Blocks[0].Lines[^2].Text;
        }

        item.Definition = GetDefinition(itemDefinitionProvider.Definitions, item.Properties.Rarity, item.ItemClass?.Id, item.Type, item.Name)!;
        if (item.Definition == null) throw new UnparsableException(item.Text.Text);

        // Poe.ninja does not include item class in the text, so we fill it here in case.
        if (item.Definition.BaseItemIds != null && item.ItemClass == null!)
        {
            var baseItem = baseItemProvider.Definitions.FirstOrDefault(x => item.Definition.BaseItemIds.Contains(x.Id));
            item.ItemClass ??= itemClassProvider.Definitions.FirstOrDefault(x => x.Id == baseItem?.ItemClassId)!;
        }
        if (item.ItemClass == null) throw new UnparsableException(item.Text.Text);

        ParseVaalGem();

        item.TradeItem = GetTradeItem(item.Definition.TradeItems, item.Type);

        if (currentGameLanguage.IsEnglish()) item.InvariantDefinition = item.Definition;
        else if (item.Definition.UniqueIds != null && item.Definition.UniqueIds.Count != 0) item.InvariantDefinition = itemDefinitionProvider.InvariantDictionary.GetValueOrDefault(item.Definition.UniqueIds.First());
        else if (item.Definition.BaseItemIds != null && item.Definition.BaseItemIds.Count != 0) item.InvariantDefinition = itemDefinitionProvider.InvariantDictionary.GetValueOrDefault(item.Definition.BaseItemIds.First());

        return;

        void ParseVaalGem()
        {
            var canBeVaalGem = item.ItemClass.Type == ItemClass.ActiveSkillGem && item.Text.Blocks.Count > 7;
            if (!canBeVaalGem || item.Text.Blocks[5].Lines.Count <= 0) return;

            var vaalGem = GetDefinition(itemDefinitionProvider.Definitions, item.Properties.Rarity, item.ItemClass.Id, item.Text.Blocks[5].Lines[0].Text, item.Name);
            if (vaalGem != null)
            {
                item.Definition = vaalGem;
            }
        }
    }

    public ItemDefinition? GetInvariant(ApiItem item)
    {
        return GetDefinition(itemDefinitionProvider.InvariantDefinitions, item.Rarity, null, item.TypeLine, item.Name);
    }

    public ItemDefinition? GetInvariant(string? text)
    {
        if (string.IsNullOrEmpty(text)) return null;
        text = text switch
        {
            "exalt" => "exalted",
            _ => text,
        };
        return itemDefinitionProvider.InvariantDictionary.GetValueOrDefault(text);
    }

    private ItemDefinition? GetDefinition(List<ItemDefinition> definitions, Rarity rarity, string? itemClassId, string? type, string? name)
    {
        List<ItemDefinition> results = [];

        if (rarity == Rarity.Unique && !string.IsNullOrEmpty(name))
        {
            results.AddRange(definitions.Where(definition => definition.NamePattern != null && definition.NamePattern.IsMatch(name)));
            if (!string.IsNullOrEmpty(type))
            {
                results.RemoveAll(definition => definition.TypePattern != null && !definition.TypePattern.IsMatch(type));
            }
        }
        else if (!string.IsNullOrEmpty(type))
        {
            results.AddRange(definitions.Where(definition => !definition.IsUnique && definition.TypePattern != null && definition.TypePattern.IsMatch(type)));
        }

        var orderedResults = results
            .Select(x =>
            {
                var ratio = 0;

                if (!string.IsNullOrEmpty(name))
                {
                    ratio += Fuzz.Ratio(x.Name, name, FuzzySharp.PreProcess.PreprocessMode.None);
                }

                if (!string.IsNullOrEmpty(type))
                {
                    ratio += Fuzz.Ratio(x.Name, type, FuzzySharp.PreProcess.PreprocessMode.None);
                }

                if (x.BaseItems.Any(baseItem => baseItem.ItemClassId == itemClassId))
                {
                    ratio += 10;
                }

                return new
                {
                    Ratio = ratio,
                    Definition = x,
                };
            })
            .OrderByDescending(x => x.Ratio);

        return orderedResults.Select(x => x.Definition).FirstOrDefault();
    }

    private TradeItem? GetTradeItem(List<TradeItem>? tradeItems, string? type)
    {
        var byType = tradeItems?.FirstOrDefault(x => x.Type == type);
        if (byType != null) return byType;

        return tradeItems?.FirstOrDefault();
    }
}
