using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class CriticalHitChanceProperty(IGameLanguageProvider gameLanguageProvider, GameType game) : PropertyDefinition
{
    private Regex Pattern { get; } = game is GameType.PathOfExile1
        ? gameLanguageProvider.Language.DescriptionCriticalStrikeChance.ToRegexDoubleCapture()
        : gameLanguageProvider.Language.DescriptionCriticalHitChance.ToRegexDoubleCapture();

    private Regex IsAugmentedPattern { get; } = game is GameType.PathOfExile1
        ? gameLanguageProvider.Language.DescriptionCriticalStrikeChance.ToRegexIsAugmented()
        : gameLanguageProvider.Language.DescriptionCriticalHitChance.ToRegexIsAugmented();

    public override List<ItemClass> ValidItemClasses { get; } = [
        ..ItemClassConstants.Weapons,
    ];

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.CriticalHitChance = GetDouble(Pattern, propertyBlock);
        if (item.Properties.CriticalHitChance == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.CriticalHitChance));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.CriticalHitChance <= 0) return Task.FromResult<TradeFilter?>(null);

        var text = game == GameType.PathOfExile1 ? gameLanguageProvider.Language.DescriptionCriticalStrikeChance : gameLanguageProvider.Language.DescriptionCriticalHitChance;
        var filter = new CriticalHitChanceFilter(game)
        {
            Text = text,
            NormalizeEnabled = true,
            Value = item.Properties.CriticalHitChance,
            ValueSuffix = "%",
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.CriticalHitChance)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class CriticalHitChanceFilter(GameType game) : DoublePropertyFilter
{
    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        switch (game)
        {
            case GameType.PathOfExile1: query.Filters.GetOrCreateWeaponFilters().Filters.CriticalHitChance = new StatFilterValue(this); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.CriticalHitChance = new StatFilterValue(this); break;
        }
    }
}
