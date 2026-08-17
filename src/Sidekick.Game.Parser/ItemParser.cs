using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Game;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Properties.Definitions;

namespace Sidekick.Apis.Poe.Trade.Parser;

public class ItemParser
(
    ILogger<ItemParser> logger,
    StatParser statParser,
    PseudoParser pseudoParser,
    PropertyParser propertyParser,
    ItemDefinitionParser itemDefinitionParser,
    ISettingsService settingsService,
    ItemClassParser itemClassParser,
    TextParser textParser
) : IInitializableService
{
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
