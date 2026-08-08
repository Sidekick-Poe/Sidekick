using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Trade.Parser.Definition;
using Sidekick.Apis.Poe.Trade.Parser.ItemClasses;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo;
using Sidekick.Apis.Poe.Trade.Parser.Stats;
using Sidekick.Apis.Poe.Trade.Parser.Text;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Extensions;
using Sidekick.Data.Items;

namespace Sidekick.Apis.Poe.Trade.Parser;

public class ItemParser
(
    ILogger<ItemParser> logger,
    IStatParser statParser,
    IPseudoParser pseudoParser,
    IPropertyParser propertyParser,
    IItemDefinitionParser itemDefinitionParser,
    ISettingsService settingsService,
    ItemClassParser itemClassParser,
    TextParser textParser
) : IItemParser
{
    public int Priority => 100;

    private GameType Game { get; set; }

    public async Task Initialize()
    {
        Game = await settingsService.GetGame();
    }

    public Item ParseItem(string? text)
    {
        if (string.IsNullOrEmpty(text)) throw new UnparsableException(text);

        try
        {
            var rawText = textParser.NormalizeText(text);
            if (rawText == null) throw new UnparsableException(text);

            var item = new Item(Game, rawText);

            // Rarity property is required for definition parsing. This means that it must be parsed first.
            propertyParser.GetDefinition<RarityProperty>().Parse(item);

            itemClassParser.Parse(item);
            itemDefinitionParser.Parse(item);
            propertyParser.Parse(item);
            statParser.Parse(item);
            propertyParser.ParseAfterStats(item);
            pseudoParser.Parse(item);

            return item;
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Could not parse item.");
            throw new UnparsableException(text);
        }
    }
}
