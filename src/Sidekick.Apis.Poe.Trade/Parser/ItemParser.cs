using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Items;
using Sidekick.Apis.Poe.Trade.Parser.Headers;
using Sidekick.Apis.Poe.Trade.Parser.Modifiers;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo;
using Sidekick.Apis.Poe.Trade.Parser.Requirements;
using Sidekick.Common.Exceptions;

namespace Sidekick.Apis.Poe.Trade.Parser;

public class ItemParser
(
    ILogger<ItemParser> logger,
    IModifierParser modifierParser,
    IPseudoParser pseudoParser,
    IRequirementsParser requirementsParser,
    IApiInvariantItemProvider apiInvariantItemProvider,
    IPropertyParser propertyParser,
    IGameLanguageProvider gameLanguageProvider,
    IHeaderParser headerParser
) : IItemParser
{
    private Regex? UnusablePattern { get; set; }

    public int Priority => 100;

    public Task Initialize()
    {
        var unusableRegex = Regex.Escape(gameLanguageProvider.Language.DescriptionUnusable);
        unusableRegex += @"[\n\r]+" + ParsingItem.SeparatorPattern + @"[\n\r]+";
        UnusablePattern = new Regex(unusableRegex, RegexOptions.Compiled);

        return Task.CompletedTask;
    }

    public Item ParseItem(string? itemText)
    {
        if (string.IsNullOrEmpty(itemText))
        {
            throw new UnparsableException(itemText);
        }

        try
        {
            itemText = RemoveUnusableLine(itemText);
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
            };
        }
        catch (UnparsableException e)
        {
            logger.LogWarning(e, "Could not parse item.");
            throw;
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Could not parse item.");
            throw new UnparsableException(itemText);
        }
    }

    private string RemoveUnusableLine(string itemText)
    {
        return UnusablePattern?.Replace(itemText, string.Empty) ?? itemText;
    }
}
