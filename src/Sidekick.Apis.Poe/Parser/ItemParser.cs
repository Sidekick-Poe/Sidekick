using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Parser.AdditionalInformation;
using Sidekick.Apis.Poe.Parser.Headers;
using Sidekick.Apis.Poe.Parser.Modifiers;
using Sidekick.Apis.Poe.Parser.Properties;
using Sidekick.Apis.Poe.Parser.Pseudo;
using Sidekick.Apis.Poe.Parser.Requirements;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Items.AdditionalInformation;

namespace Sidekick.Apis.Poe.Parser;

public class ItemParser
(
    ILogger<ItemParser> logger,
    IModifierParser modifierParser,
    IPseudoParser pseudoParser,
    IRequirementsParser requirementsParser,
    ClusterJewelParser clusterJewelParser,
    IApiInvariantItemProvider apiInvariantItemProvider,
    IPropertyParser propertyParser,
    IHeaderParser headerParser
) : IItemParser
{
    public Item ParseItem(string? itemText)
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
            requirementsParser.Parse(parsingItem);
            var properties = propertyParser.Parse(parsingItem);
            var modifierLines = modifierParser.Parse(parsingItem);
            propertyParser.ParseAfterModifiers(parsingItem, properties, modifierLines);
            var pseudoModifiers = pseudoParser.Parse(modifierLines);

            return new Item()
            {
                Invariant = invariant,
                Header = parsingItem.Header,
                Properties = properties,
                ModifierLines = modifierLines,
                PseudoModifiers = pseudoModifiers,
                Text = parsingItem.Text,
                AdditionalInformation = ParseAdditionalInformation(parsingItem.Header, modifierLines),
            };
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Could not parse item.");
            throw;
        }
    }

    private ClusterJewelInformation? ParseAdditionalInformation(ItemHeader itemHeader, List<ModifierLine> modifierLines)
    {
        if (clusterJewelParser.TryParse(itemHeader, modifierLines, out var clusterJewelInformation))
        {
            return clusterJewelInformation;
        }

        return null;
    }
}
