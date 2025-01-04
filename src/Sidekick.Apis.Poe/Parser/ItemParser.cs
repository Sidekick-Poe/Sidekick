using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Parser.AdditionalInformation;
using Sidekick.Apis.Poe.Parser.Headers;
using Sidekick.Apis.Poe.Parser.Modifiers;
using Sidekick.Apis.Poe.Parser.Properties;
using Sidekick.Apis.Poe.Parser.Pseudo;
using Sidekick.Apis.Poe.Parser.Requirements;
using Sidekick.Apis.Poe.Parser.Sockets;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Parser;

public class ItemParser
(
    ILogger<ItemParser> logger,
    IModifierParser modifierParser,
    IPseudoParser pseudoParser,
    IRequirementsParser requirementsParser,
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
            requirementsParser.Parse(parsingItem);
            var sockets = socketParser.Parse(parsingItem);
            var properties = propertyParser.Parse(parsingItem);
            var modifierLines = modifierParser.Parse(parsingItem);
            propertyParser.ParseAfterModifiers(parsingItem, properties, modifierLines);
            var pseudoModifiers = pseudoParser.Parse(modifierLines);
            var item = new Item(invariant: invariant,
                                itemHeader: parsingItem.Header,
                                itemProperties: properties,
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
}
