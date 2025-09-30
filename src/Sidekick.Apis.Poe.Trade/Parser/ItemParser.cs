using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Items;
using Sidekick.Apis.Poe.Trade.Parser.Modifiers;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;
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
    IPropertyParser propertyParser,
    IGameLanguageProvider gameLanguageProvider,
    IApiItemProvider apiItemProvider,
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

    public Item ParseItem(string? text)
    {
        if (string.IsNullOrEmpty(text)) throw new UnparsableException(text);

        try
        {
            text = RemoveUnusableLine(text);

            var item = new Item(Game, text);

            // These properties are required for later parsing steps
            propertyParser.GetDefinition<ItemClassProperty>().Parse(item);
            propertyParser.GetDefinition<RarityProperty>().Parse(item);

            item.ApiInformation = apiItemProvider.GetApiItem(item.Properties.Rarity, item.Name, item.Type) ?? throw new UnparsableException(item.Text.Text);
            ParseVaalGem(item);

            requirementsParser.Parse(item.Text);
            propertyParser.Parse(item);
            modifierParser.Parse(item);
            propertyParser.ParseAfterModifiers(item);
            pseudoParser.Parse(item);

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

    private void ParseVaalGem(Item item)
    {
        var canBeVaalGem = item.Properties.ItemClass == ItemClass.ActiveGem && item.Text.Blocks.Count > 7;
        if (!canBeVaalGem || item.Text.Blocks[5].Lines.Count <= 0) return;

        if (apiItemProvider.NameAndTypeDictionary.TryGetValue(item.Text.Blocks[5].Lines[0].Text, out var apiItems) && apiItems.Count > 0)
        {
            item.ApiInformation = apiItems.First();
        }
    }
}
