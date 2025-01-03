using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Parser.AdditionalInformation;
using Sidekick.Apis.Poe.Parser.Headers;
using Sidekick.Apis.Poe.Parser.Modifiers;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Apis.Poe.Parser.Properties;
using Sidekick.Apis.Poe.Parser.Pseudo;
using Sidekick.Apis.Poe.Parser.Sockets;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Parser;

public class ItemParser
(
    ILogger<ItemParser> logger,
    IModifierParser modifierParser,
    IPseudoParser pseudoParser,
    IParserPatterns patterns,
    ClusterJewelParser clusterJewelParser,
    IApiInvariantItemProvider apiInvariantItemProvider,
    ISocketParser socketParser,
    IPropertyParser propertyParser,
    IHeaderParser headerParser
) : IItemParser
{
    public Task<Item> ParseItemAsync(string itemText)
    {
        return Task.Run(() => ParseItem(itemText));
    }

    public Item ParseItem(string itemText)
    {
        if (string.IsNullOrEmpty(itemText))
        {
            throw new UnparsableException();
        }

        try
        {
            var parsingItem = new ParsingItem(itemText);
            parsingItem.Header = headerParser.Parse(parsingItem);
            if (parsingItem.Header == null || (string.IsNullOrEmpty(parsingItem.Header.ApiName) && string.IsNullOrEmpty(parsingItem.Header.ApiType)))
            {
                throw new UnparsableException();
            }

            ItemHeader? invariant = null;
            if (parsingItem.Header.ApiItemId != null && apiInvariantItemProvider.IdDictionary.TryGetValue(parsingItem.Header.ApiItemId, out var invariantMetadata))
            {
                invariant = invariantMetadata.ToHeader();
            }

            // Order of parsing is important
            ParseRequirements(parsingItem);

            var influences = ParseInfluences(parsingItem);
            var sockets = socketParser.Parse(parsingItem);
            var properties = propertyParser.Parse(parsingItem);
            var modifierLines = ParseModifiers(parsingItem);
            propertyParser.ParseAfterModifiers(parsingItem, properties, modifierLines);
            var pseudoModifiers = pseudoParser.Parse(modifierLines);
            var item = new Item(invariant: invariant,
                                itemHeader: parsingItem.Header,
                                itemProperties: properties,
                                influences: influences,
                                sockets: sockets,
                                modifierLines: modifierLines,
                                pseudoModifiers: pseudoModifiers,
                                text: parsingItem.Text);

            if (clusterJewelParser.TryParse(item, out var clusterInformation))
            {
                item.AdditionalInformation = clusterInformation;
            }

            return item;
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Could not parse item.");
            throw;
        }
    }

    private void ParseRequirements(ParsingItem parsingItem)
    {
        foreach (var block in parsingItem.Blocks.Where(x => !x.Parsed))
        {
            if (!block.TryParseRegex(patterns.Requirements, out _))
            {
                continue;
            }

            block.Parsed = true;
            return;
        }
    }

    private Influences ParseInfluences(ParsingItem parsingItem)
    {
        return parsingItem.Header?.Category switch
        {
            Category.Accessory or Category.Armour or Category.Weapon => new Influences()
            {
                Crusader = GetBool(patterns.Crusader, parsingItem),
                Elder = GetBool(patterns.Elder, parsingItem),
                Hunter = GetBool(patterns.Hunter, parsingItem),
                Redeemer = GetBool(patterns.Redeemer, parsingItem),
                Shaper = GetBool(patterns.Shaper, parsingItem),
                Warlord = GetBool(patterns.Warlord, parsingItem),
            },
            _ => new Influences(),
        };
    }

    private List<ModifierLine> ParseModifiers(ParsingItem parsingItem)
    {
        return parsingItem.Header?.Category switch
        {
            Category.DivinationCard or Category.Gem => new(),
            _ => modifierParser.Parse(parsingItem),
        };
    }

    #region Helpers

    private static bool GetBool(Regex pattern, ParsingItem parsingItem)
    {
        return parsingItem.TryParseRegex(pattern, out _);
    }

    #endregion Helpers
}
