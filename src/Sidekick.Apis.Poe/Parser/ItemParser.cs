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
            var header = headerParser.Parse(parsingItem);

            ItemHeader? invariant = null;
            if (header.ApiItemId != null && apiInvariantItemProvider.IdDictionary.TryGetValue(header.ApiItemId, out var invariantMetadata))
            {
                invariant = invariantMetadata.ToHeader();
            }

            // Order of parsing is important
            requirementsParser.Parse(parsingItem);
            var properties = propertyParser.Parse(parsingItem, header);
            var modifierLines = modifierParser.Parse(parsingItem, header);
            propertyParser.ParseAfterModifiers(parsingItem, header, properties, modifierLines);
            var pseudoModifiers = pseudoParser.Parse(modifierLines);

            return new Item()
            {
                Invariant = invariant,
                Header = header,
                Properties = properties,
                ModifierLines = modifierLines,
                PseudoModifiers = pseudoModifiers,
                Text = parsingItem.Text,
                AdditionalInformation = ParseAdditionalInformation(header, modifierLines),
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
