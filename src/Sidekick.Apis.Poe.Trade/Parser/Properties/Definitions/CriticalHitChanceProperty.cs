using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class CriticalHitChanceProperty(IGameLanguageProvider gameLanguageProvider, GameType game) : PropertyDefinition
{
    private Regex Pattern { get; } = game is GameType.PathOfExile
        ? gameLanguageProvider.Language.DescriptionCriticalStrikeChance.ToRegexDoubleCapture()
        : gameLanguageProvider.Language.DescriptionCriticalHitChance.ToRegexDoubleCapture();

    private Regex IsAugmentedPattern { get; } = game is GameType.PathOfExile
        ? gameLanguageProvider.Language.DescriptionCriticalStrikeChance.ToRegexIsAugmented()
        : gameLanguageProvider.Language.DescriptionCriticalHitChance.ToRegexIsAugmented();

    public override List<Category> ValidCategories { get; } = [Category.Weapon];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem, ItemHeader header)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.CriticalHitChance = GetDouble(Pattern, propertyBlock);
        if (itemProperties.CriticalHitChance == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) itemProperties.AugmentedProperties.Add(nameof(ItemProperties.CriticalHitChance));
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.CriticalHitChance <= 0) return null;

        var text = game == GameType.PathOfExile ? gameLanguageProvider.Language.DescriptionCriticalStrikeChance : gameLanguageProvider.Language.DescriptionCriticalHitChance;
        var filter = new DoublePropertyFilter(this)
        {
            Text = text,
            NormalizeEnabled = true,
            NormalizeValue = normalizeValue,
            Value = item.Properties.CriticalHitChance,
            ValueSuffix = "%",
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.CriticalHitChance)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        filter.ChangeFilterType(filterType);
        return filter;
    }

    public override void PrepareTradeRequest(Query query, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not DoublePropertyFilter doubleFilter) return;

        switch (game)
        {
            case GameType.PathOfExile: query.Filters.GetOrCreateWeaponFilters().Filters.CriticalHitChance = new StatFilterValue(doubleFilter); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.CriticalHitChance = new StatFilterValue(doubleFilter); break;
        }
    }
}
