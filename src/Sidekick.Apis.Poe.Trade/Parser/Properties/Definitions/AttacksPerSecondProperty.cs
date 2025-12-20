using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class AttacksPerSecondProperty
(
    IGameLanguageProvider gameLanguageProvider,
    GameType game
) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionAttacksPerSecond.ToRegexDoubleCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionAttacksPerSecond.ToRegexIsAugmented();

    public override List<ItemClass> ValidItemClasses { get; } = [
        ..ItemClassConstants.Weapons,
    ];

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.AttacksPerSecond = GetDouble(Pattern, propertyBlock);
        if (item.Properties.AttacksPerSecond == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.AttacksPerSecond));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.AttacksPerSecond <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new DoublePropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionAttacksPerSecond,
            NormalizeEnabled = true,
            Value = item.Properties.AttacksPerSecond,
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.AttacksPerSecond)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, TradeFilter filter)
    {
        if (!filter.Checked || filter is not DoublePropertyFilter doubleFilter) return;

        switch (game)
        {
            case GameType.PathOfExile1: query.Filters.GetOrCreateWeaponFilters().Filters.AttacksPerSecond = new StatFilterValue(doubleFilter); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.AttacksPerSecond = new StatFilterValue(doubleFilter); break;
        }
    }
}
