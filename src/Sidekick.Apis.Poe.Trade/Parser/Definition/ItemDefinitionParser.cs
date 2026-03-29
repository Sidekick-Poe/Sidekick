using FuzzySharp;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Items.Models;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Extensions;
using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Languages;
namespace Sidekick.Apis.Poe.Trade.Parser.Definition;

public class ItemDefinitionParser(
    DataProvider dataProvider,
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService
) : IItemDefinitionParser
{
    private Dictionary<string, ItemDefinition> TextDictionary { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, ItemDefinition> InvariantDictionary { get; } = new(StringComparer.Ordinal);

    public List<ItemDefinition> Definitions { get; private set; } = [];
    public List<ItemDefinition> InvariantDefinitions { get; private set; } = [];
    public List<ItemDefinition> UniqueItems { get; private set; } = [];

    public int Priority => 100;

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();

        Definitions = await dataProvider.Read<List<ItemDefinition>>(game, DataType.Items, currentGameLanguage.Language);
        InvariantDefinitions = await dataProvider.Read<List<ItemDefinition>>(game, DataType.Items, currentGameLanguage.InvariantLanguage);
        UniqueItems = Definitions.Where(x => x.UniqueItem != null)
            .OrderByDescending(x => x.UniqueItem?.Name?.Length ?? 0)
            .ToList();

        TextDictionary.Clear();
        foreach (var definition in Definitions)
        {
            if (!string.IsNullOrEmpty(definition.TradeItem?.Name)) TextDictionary.TryAdd(definition.TradeItem.Name, definition);
            else if (!string.IsNullOrEmpty(definition.TradeItem?.Text)) TextDictionary.TryAdd(definition.TradeItem.Text, definition);
            else if (!string.IsNullOrEmpty(definition.TradeItem?.Type)) TextDictionary.TryAdd(definition.TradeItem.Type, definition);
            if (!string.IsNullOrEmpty(definition.TradeItem?.Id)) TextDictionary.TryAdd(definition.TradeItem.Id, definition);
        }

        BuildInvariantDictionary();

        return;

        void BuildInvariantDictionary()
        {
            InvariantDictionary.Clear();

            foreach (var definition in InvariantDefinitions)
            {
                if (string.IsNullOrEmpty(definition.Key)) continue;

                InvariantDictionary.TryAdd(definition.Key, definition);
            }
        }
    }

    public void Parse(Item item)
    {
        item.Definition = GetDefinition(Definitions, item.Type, item.Properties.Rarity, item.Name) ?? throw new UnparsableException(item.Text.Text);
        item.Invariant = GetInvariant(item.Definition) ?? throw new UnparsableException(item.Text.Text);
        ParseVaalGem();

        return;

        void ParseVaalGem()
        {
            var canBeVaalGem = item.ItemClass == ItemClass.ActiveSkillGem && item.Text.Blocks.Count > 7;
            if (!canBeVaalGem || item.Text.Blocks[5].Lines.Count <= 0) return;

            var vaalGem = GetDefinition(Definitions, item.Text.Blocks[5].Lines[0].Text, item.Properties.Rarity, item.Name);
            if (vaalGem != null)
            {
                item.Definition = vaalGem;
            }
        }

        ItemDefinition? GetInvariant(ItemDefinition definition)
        {
            if (currentGameLanguage.Language.Code == currentGameLanguage.InvariantLanguage.Code) return definition;
            if (string.IsNullOrEmpty(definition.Key)) return null;
            return InvariantDictionary.GetValueOrDefault(definition.Key);
        }
    }

    public ItemDefinition? Get(ApiItem apiItem)
    {
        return GetDefinition(InvariantDefinitions, apiItem.Type, apiItem.Rarity, apiItem.Name);
    }

    private ItemDefinition? GetDefinition(List<ItemDefinition> definitions, string? type, Rarity rarity, string? name)
    {
        if (rarity == Rarity.Unique && !string.IsNullOrEmpty(name))
        {
            var results = definitions.Where(definition => definition.NamePattern != null && definition.NamePattern.IsMatch(name));
            var bestMatch = FindBestMatch(results, x => x.TradeItem?.Text ?? x.TradeItem?.Name, $"{name} {type}");
            if (bestMatch != null) return bestMatch;
        }

        if (!string.IsNullOrEmpty(type))
        {
            var textResults = definitions.Where(definition => definition.TextPattern != null && definition.TextPattern.IsMatch(type));
            var textMatch = FindBestMatch(textResults, x => x.TradeItem?.Text, type);
            if (textMatch != null) return textMatch;
        }

        if (!string.IsNullOrEmpty(type))
        {
            var typeResults = definitions.Where(definition => definition.TypePattern != null && definition.TypePattern.IsMatch(type));
            var typeMatch = FindBestMatch(typeResults, x => x.BaseItem?.Name ?? x.TradeItem?.Type, type);
            if (typeMatch != null) return typeMatch;
        }

        return null;
    }

    private ItemDefinition? FindBestMatch(IEnumerable<ItemDefinition> definitions, Func<ItemDefinition, string?> compareFunc, string text)
    {
        return definitions
            .Select(x =>
            {
                var compare = compareFunc(x);
                var ratio = 0;
                if (!string.IsNullOrEmpty(compare)) ratio = Fuzz.Ratio(text, compare, FuzzySharp.PreProcess.PreprocessMode.None);

                return new
                {
                    Ratio = ratio,
                    Definition = x,
                };
            })
            .OrderByDescending(x => x.Ratio)
            .Select(x => x.Definition)
            .FirstOrDefault();
    }

    public ItemDefinition? Get(string? text)
    {
        if (string.IsNullOrEmpty(text)) return null;
        text = text switch
        {
            "exalt" => "exalted",
            _ => text,
        };
        return TextDictionary.GetValueOrDefault(text);
    }
}
