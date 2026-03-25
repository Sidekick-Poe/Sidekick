using FuzzySharp;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.ApiItems;
using Sidekick.Common.Exceptions;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
namespace Sidekick.Apis.Poe.Trade.Parser.Definition;

public class ItemDefinitionParser(
    IApiItemProvider apiItemProvider,
    ICurrentGameLanguage currentGameLanguage) : IItemDefinitionParser
{
    public void Parse(Item item)
    {
        item.Definition = GetDefinition(item.Type) ?? throw new UnparsableException(item.Text.Text);
        item.Invariant = GetInvariant(item.Definition) ?? throw new UnparsableException(item.Text.Text);
        ParseVaalGem();

        return;

        ItemDefinition? GetDefinition(string? type)
        {
            if (item.Properties.Rarity == Rarity.Unique && !string.IsNullOrEmpty(item.Name))
            {
                var results = apiItemProvider.Definitions.Where(definition => definition.NamePattern != null && definition.NamePattern.IsMatch(item.Name));
                var bestMatch = FindBestMatch(results, x => x.TradeItem?.Text ?? x.TradeItem?.Name, $"{item.Name} {item.Type}");
                if (bestMatch != null) return bestMatch;
            }

            if (!string.IsNullOrEmpty(type))
            {
                var textResults = apiItemProvider.Definitions.Where(definition => definition.TextPattern != null && definition.TextPattern.IsMatch(type));
                var textMatch = FindBestMatch(textResults, x => x.TradeItem?.Text, type);
                if (textMatch != null) return textMatch;
            }

            if (!string.IsNullOrEmpty(type))
            {
                var typeResults = apiItemProvider.Definitions.Where(definition => definition.TypePattern != null && definition.TypePattern.IsMatch(type));
                var typeMatch = FindBestMatch(typeResults, x => x.TradeItem?.Type, type);
                if (typeMatch != null) return typeMatch;
            }

            return null;
        }

        ItemDefinition? FindBestMatch(IEnumerable<ItemDefinition> definitions, Func<ItemDefinition, string?> compareFunc, string text)
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

        void ParseVaalGem()
        {
            // todo vaal
            // var canBeVaalGem = item.ItemClass == ItemClass.ActiveGem && item.Text.Blocks.Count > 7;
            // if (!canBeVaalGem || item.Text.Blocks[5].Lines.Count <= 0) return;
//
            // var vaalGem = GetDefinition(item.Text.Blocks[5].Lines[0].Text);
            // if (vaalGem != null)
            // {
            //     item.Definition = vaalGem;
            // }
        }

        ItemDefinition? GetInvariant(ItemDefinition definition)
        {
            if (currentGameLanguage.Language.Code == currentGameLanguage.InvariantLanguage.Code) return definition;
            if (string.IsNullOrEmpty(definition.Key)) return null;
            return apiItemProvider.InvariantDictionary.GetValueOrDefault(definition.Key);
        }
    }
}
