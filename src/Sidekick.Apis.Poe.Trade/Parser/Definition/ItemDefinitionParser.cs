using FuzzySharp;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Extensions;
using Sidekick.Data.ItemClasses;
using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
namespace Sidekick.Apis.Poe.Trade.Parser.Definition;

public class ItemDefinitionParser(
    DataProvider dataProvider,
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService
) : IItemDefinitionParser
{
    public Dictionary<string, ItemDefinition> InvariantDictionary { get; } = new(StringComparer.Ordinal);

    private List<ItemDefinition> Definitions { get; set; } = [];
    private List<ItemDefinition> InvariantDefinitions { get; set; } = [];
    public List<ItemDefinition> UniqueItems { get; private set; } = [];

    public int Priority => 100;

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();

        Definitions = await dataProvider.Read<List<ItemDefinition>>(game, DataType.Items, currentGameLanguage.Language);
        UniqueItems = Definitions.Where(x => x.IsUnique)
            .OrderByDescending(x => x.Name?.Length ?? 0)
            .ToList();

        InvariantDefinitions = await dataProvider.Read<List<ItemDefinition>>(game, DataType.Items, currentGameLanguage.InvariantLanguage);
        InvariantDictionary.Clear();
        foreach (var definition in InvariantDefinitions)
        {
            if (string.IsNullOrEmpty(definition.InvariantKey)) continue;

            InvariantDictionary.TryAdd(definition.InvariantKey, definition);
        }
    }

    public void Parse(Item item)
    {
        item.Type = item.Text.Blocks[0].Lines[^1].Text;
        if (item.Properties.Rarity is Rarity.Rare or Rarity.Unique && item.Text.Blocks[0].Lines.Count >= 4)
        {
            item.Name = item.Text.Blocks[0].Lines[^2].Text;
        }

        var definition = GetDefinition(item.Type, item.Properties.Rarity, item.Name);
        if (definition == null) throw new UnparsableException(item.Text.Text);

        item.Definition = definition;
        ParseVaalGem();

        item.ExchangeItem = item.Definition.ExchangeItem;
        item.TradeItem = GetTradeItem(item);

        return;

        void ParseVaalGem()
        {
            var canBeVaalGem = item.ItemClass.Type == ItemClass.ActiveSkillGem && item.Text.Blocks.Count > 7;
            if (!canBeVaalGem || item.Text.Blocks[5].Lines.Count <= 0) return;

            var vaalGem = GetDefinition(item.Text.Blocks[5].Lines[0].Text, item.Properties.Rarity, item.Name);
            if (vaalGem != null)
            {
                item.Definition = vaalGem;
            }
        }
    }

    private ItemDefinition? GetInvariant(ItemDefinition definition)
    {
        if (currentGameLanguage.Language.Code == currentGameLanguage.InvariantLanguage.Code) return definition;
        if (string.IsNullOrEmpty(definition.InvariantKey)) return null;
        return InvariantDictionary.GetValueOrDefault(definition.InvariantKey);
    }

    private ItemDefinition? GetDefinition(string? type, Rarity rarity, string? name)
    {
        List<ItemDefinition> results = [];

        if (rarity == Rarity.Unique && !string.IsNullOrEmpty(name))
        {
            results.AddRange(Definitions.Where(definition => definition.NamePattern != null && definition.NamePattern.IsMatch(name)));
            if (!string.IsNullOrEmpty(type))
            {
                results.RemoveAll(definition => definition.TypePattern != null && !definition.TypePattern.IsMatch(type));
            }
        }
        else if (!string.IsNullOrEmpty(type))
        {
            results.AddRange(Definitions.Where(definition => !definition.IsUnique && definition.TypePattern != null && definition.TypePattern.IsMatch(type)));
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

                return new
                {
                    Ratio = ratio,
                    Definition = x,
                };
            })
            .OrderByDescending(x => x.Ratio);

        return orderedResults.Select(x => x.Definition).FirstOrDefault();
    }

    private TradeItemDefinition? GetTradeItem(Item item)
    {
        var tradeItems = item.Definition.TradeItems;

        var byType = tradeItems?.FirstOrDefault(x => x.Type == item.Type);
        if (byType != null) return byType;

        return tradeItems?.FirstOrDefault();
    }

    public ItemDefinition? GetInvariant(string? text)
    {
        if (string.IsNullOrEmpty(text)) return null;
        text = text switch
        {
            "exalt" => "exalted",
            _ => text,
        };
        return InvariantDictionary.GetValueOrDefault(text);
    }
}
