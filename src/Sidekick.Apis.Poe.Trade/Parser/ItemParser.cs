using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Items;
using Sidekick.Apis.Poe.Trade.Parser.Headers;
using Sidekick.Apis.Poe.Trade.Parser.Modifiers;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo;
using Sidekick.Apis.Poe.Trade.Parser.Requirements;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;

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
    IHeaderParser headerParser,
    ISettingsService settingsService
) : IItemParser
{
    private Regex? UnusablePattern { get; set; }

    public int Priority => 100;

    private GameType Game { get; set; }

    public async Task Initialize()
    {
        var unusableRegex = Regex.Escape(gameLanguageProvider.Language.DescriptionUnusable);
        unusableRegex += @"[\n\r]+" + TextItem.SeparatorPattern + @"[\n\r]+";
        UnusablePattern = new Regex(unusableRegex, RegexOptions.Compiled);
        Game = await settingsService.GetGame();
    }

    public Item ParseItem(string? text, string? advancedText = null)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new UnparsableException(text);
        }

        try
        {
            text = RemoveUnusableLine(text);
            var item = new Item(Game, text, advancedText);

            headerParser.Parse(item);

            if (item.Header.ApiItemId != null && apiInvariantItemProvider.IdDictionary.TryGetValue(item.Header.ApiItemId, out var invariantMetadata))
            {
                item.Invariant = invariantMetadata.ToHeader();
            }

            // Order of parsing is important
            requirementsParser.Parse(item.Text);

            propertyParser.Parse(item);
            modifierParser.Parse(item);
            propertyParser.ParseAfterModifiers(item);
            pseudoParser.Parse(item);

            headerParser.Parse(item);

            return item;
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Could not parse item.");
            throw new UnparsableException(text);
        }
    }

    private string RemoveUnusableLine(string itemText)
    {
        return UnusablePattern?.Replace(itemText, string.Empty) ?? itemText;
    }
}
