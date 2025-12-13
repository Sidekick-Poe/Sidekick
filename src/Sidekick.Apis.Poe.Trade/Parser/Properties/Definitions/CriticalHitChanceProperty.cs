using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class CriticalHitChanceProperty(IGameLanguageProvider gameLanguageProvider, GameType game) : PropertyDefinition
{
    private Regex Pattern { get; } = game is GameType.PathOfExile1
        ? gameLanguageProvider.Language.DescriptionCriticalStrikeChance.ToRegexDoubleCapture()
        : gameLanguageProvider.Language.DescriptionCriticalHitChance.ToRegexDoubleCapture();

    private Regex IsAugmentedPattern { get; } = game is GameType.PathOfExile1
        ? gameLanguageProvider.Language.DescriptionCriticalStrikeChance.ToRegexIsAugmented()
        : gameLanguageProvider.Language.DescriptionCriticalHitChance.ToRegexIsAugmented();

    public override List<Category> ValidItemClasses { get; } = [Category.Weapon];

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.CriticalHitChance = GetDouble(Pattern, propertyBlock);
        if (item.Properties.CriticalHitChance == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.CriticalHitChance));
    }

    public override Task<PropertyFilter?> GetFilter(Item item)
    {
        if (item.Properties.CriticalHitChance <= 0) return Task.FromResult<PropertyFilter?>(null);

        var text = game == GameType.PathOfExile1 ? gameLanguageProvider.Language.DescriptionCriticalStrikeChance : gameLanguageProvider.Language.DescriptionCriticalHitChance;
        var filter = new DoublePropertyFilter(this)
        {
            Text = text,
            NormalizeEnabled = true,
            Value = item.Properties.CriticalHitChance,
            ValueSuffix = "%",
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.CriticalHitChance)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked || filter is not DoublePropertyFilter doubleFilter) return;

        switch (game)
        {
            case GameType.PathOfExile1: query.Filters.GetOrCreateWeaponFilters().Filters.CriticalHitChance = new StatFilterValue(doubleFilter); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.CriticalHitChance = new StatFilterValue(doubleFilter); break;
        }
    }
}
