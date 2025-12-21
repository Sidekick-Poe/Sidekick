using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class BlockChanceProperty(IGameLanguageProvider gameLanguageProvider, GameType game) : PropertyDefinition
{
    private Regex Pattern { get; } = game is GameType.PathOfExile1
        ? gameLanguageProvider.Language.DescriptionChanceToBlock.ToRegexIntCapture()
        : gameLanguageProvider.Language.DescriptionBlockChance.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = game is GameType.PathOfExile1
        ? gameLanguageProvider.Language.DescriptionChanceToBlock.ToRegexIsAugmented()
        : gameLanguageProvider.Language.DescriptionBlockChance.ToRegexIsAugmented();

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Equipment,
    ];

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.BlockChance = GetInt(Pattern, propertyBlock);
        if (item.Properties.BlockChance == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.BlockChance));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.BlockChance <= 0) return Task.FromResult<TradeFilter?>(null);

        var text = game == GameType.PathOfExile1 ? gameLanguageProvider.Language.DescriptionChanceToBlock : gameLanguageProvider.Language.DescriptionBlockChance;
        var filter = new BlockChanceFilter(game)
        {
            Text = text,
            NormalizeEnabled = true,
            Value = item.Properties.BlockChance,
            ValueSuffix = "%",
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.BlockChance)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class BlockChanceFilter(GameType game) : IntPropertyFilter
{
    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        switch (game)
        {
            case GameType.PathOfExile1: query.Filters.GetOrCreateArmourFilters().Filters.BlockChance = new StatFilterValue(this); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.BlockChance = new StatFilterValue(this); break;
        }
    }
}
