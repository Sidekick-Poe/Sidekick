using FuzzySharp;
using Sidekick.Apis.Poe.Trade.Trade.Models;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game;
using Sidekick.Game.ItemClasses;
using Sidekick.Game.ItemDefinitions;
using Sidekick.Game.Parser.Items;
namespace Sidekick.Apis.Poe.Trade.Parser;

public class ItemDefinitionParser(
    DataProvider dataProvider,
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService,
    ItemClassParser itemClassParser
) : IInitializableService
{
    public Dictionary<string, ItemDefinition> InvariantDictionary { get; } = new(StringComparer.Ordinal);

    private List<ItemDefinition> Definitions { get; set; } = [];
    private List<ItemDefinition> InvariantDefinitions { get; set; } = [];
    public List<ItemDefinition> UniqueItems { get; private set; } = [];

    public int Priority => 100;

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();

        Definitions = await dataProvider.Read<List<ItemDefinition>>(game, GameDataType.Items, currentGameLanguage.Language);
        UniqueItems = Definitions.Where(x => x.IsUnique)
            .OrderByDescending(x => x.Name?.Length ?? 0)
            .ToList();

        InvariantDictionary.Clear();
        if (currentGameLanguage.IsEnglish()) InvariantDefinitions = Definitions;
        else InvariantDefinitions = await dataProvider.Read<List<ItemDefinition>>(game, GameDataType.Items, currentGameLanguage.InvariantLanguage);
        foreach (var definition in InvariantDefinitions)
        {
            if (string.IsNullOrEmpty(definition.Key)) continue;

            InvariantDictionary.TryAdd(definition.Key, definition);
        }
    }

    public void Parse(Item item)
    {
        item.Type = item.Text.Blocks[0].Lines[^1].Text;
        if (item.Properties.Rarity is Rarity.Rare or Rarity.Unique && item.Text.Blocks[0].Lines.Count >= 4)
        {
            item.Name = item.Text.Blocks[0].Lines[^2].Text;
        }

        item.Definition = GetDefinition(Definitions, item.Properties.Rarity, item.Type, item.Name)!;
        if (item.Definition == null) throw new UnparsableException(item.Text.Text);

        // Poe.ninja does not include item class in the text, so we fill it in here in case.
        item.ItemClass ??= itemClassParser.Definitions.FirstOrDefault(x => x.Id == item.Definition.ItemClassId)!;
        if (item.ItemClass == null) throw new UnparsableException(item.Text.Text);
        ParseVaalGem();

        item.TradeItem = GetTradeItem(item.Definition.TradeItems, item.Type);

        if (currentGameLanguage.IsEnglish()) item.InvariantDefinition = item.Definition;
        else if(item.Definition.Key != null) item.InvariantDefinition = InvariantDictionary.GetValueOrDefault(item.Definition.Key);

        return;

        void ParseVaalGem()
        {
            var canBeVaalGem = item.ItemClass.Type == ItemClass.ActiveSkillGem && item.Text.Blocks.Count > 7;
            if (!canBeVaalGem || item.Text.Blocks[5].Lines.Count <= 0) return;

            var vaalGem = GetDefinition(Definitions, item.Properties.Rarity, item.Text.Blocks[5].Lines[0].Text, item.Name);
            if (vaalGem != null)
            {
                item.Definition = vaalGem;
            }
        }
    }

    public ItemDefinition? GetInvariant(ApiItem item)
    {
        return GetDefinition(InvariantDefinitions, item.Rarity, item.TypeLine, item.Name);
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

    private ItemDefinition? GetDefinition(List<ItemDefinition> definitions, Rarity rarity, string? type, string? name)
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
