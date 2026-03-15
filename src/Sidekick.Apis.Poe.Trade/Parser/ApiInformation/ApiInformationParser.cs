using FuzzySharp;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.ApiItems;
using Sidekick.Data.Items;
using Sidekick.Common.Exceptions;
namespace Sidekick.Apis.Poe.Trade.Parser.ApiInformation;

public class ApiInformationParser(IApiItemProvider apiItemProvider) : IApiInformationParser
{
    public void Parse(Item item)
    {
        item.Definition = GetItemInformation(item.Properties.Rarity, item.Name, item.Type) ?? throw new UnparsableException(item.Text.Text);
        ParseVaalGem();

        return;

        ItemDefinition? GetItemInformation(Rarity rarity, string? name, string? type)
        {
            // Rares may have conflicting names, so we don't want to search any unique items that may have that name. Like "Ancient Orb" which can be used by abyss jewels.
            name = rarity is Rarity.Rare or Rarity.Magic ? null : name;
            if (!string.IsNullOrEmpty(name))
            {
                var nameResults = apiItemProvider.Definitions.Where(definition => definition.NamePattern != null && definition.NamePattern.IsMatch(name));
                var nameMatch = FindBestMatch(nameResults, x=>x.Name, name);
                if (nameMatch != null) return nameMatch;
            }

            if (!string.IsNullOrEmpty(type))
            {
                var textResults = apiItemProvider.Definitions.Where(definition => definition.TextPattern != null && definition.TextPattern.IsMatch(type));
                var textMatch = FindBestMatch(textResults, x=>x.Text, type);
                if (textMatch != null) return textMatch;
            }

            if (!string.IsNullOrEmpty(type))
            {
                var typeResults = apiItemProvider.Definitions.Where(definition => definition.TypePattern != null && definition.TypePattern.IsMatch(type));
                var typeMatch = FindBestMatch(typeResults, x=>x.Type, type);
                if (typeMatch != null) return typeMatch;
            }

            return null;
        }

        ItemDefinition? FindBestMatch(IEnumerable<ItemDefinition> definitions, Func<ItemDefinition, string?> definitionTextFunc, string originalText)
        {
            return definitions
                .Select(x => new
                {
                    Ratio = Fuzz.Ratio(originalText, definitionTextFunc(x), FuzzySharp.PreProcess.PreprocessMode.None),
                    Definition = x,
                })
                .OrderByDescending(x => x.Ratio)
                .Select(x => x.Definition)
                .FirstOrDefault();
        }

        void ParseVaalGem()
        {
            var canBeVaalGem = item.Properties.ItemClass == ItemClass.ActiveGem && item.Text.Blocks.Count > 7;
            if (!canBeVaalGem || item.Text.Blocks[5].Lines.Count <= 0) return;

            var vaalGem = GetItemInformation(Rarity.Gem, null, item.Text.Blocks[5].Lines[0].Text);
            if (vaalGem != null)
            {
                item.Definition = vaalGem;
            }
        }
    }
}
