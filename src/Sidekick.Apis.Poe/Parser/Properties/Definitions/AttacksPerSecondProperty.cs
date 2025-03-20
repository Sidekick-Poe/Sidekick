using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Results;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class AttacksPerSecondProperty
(
    IGameLanguageProvider gameLanguageProvider,
    GameType game
) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionAttacksPerSecond.ToRegexDoubleCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionAttacksPerSecond.ToRegexIsAugmented();

    public override List<Category> ValidCategories { get; } = [Category.Weapon];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.AttacksPerSecond = GetDouble(Pattern, propertyBlock);
        if (itemProperties.AttacksPerSecond == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) itemProperties.AugmentedProperties.Add(nameof(ItemProperties.AttacksPerSecond));
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.AttacksPerSecond <= 0) return null;

        var filter = new DoublePropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionAttacksPerSecond,
            NormalizeEnabled = true,
            NormalizeValue = normalizeValue,
            Value = item.Properties.AttacksPerSecond,
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.AttacksPerSecond)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        filter.ChangeFilterType(filterType);
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not DoublePropertyFilter doubleFilter) return;

        switch (game)
        {
            case GameType.PathOfExile: searchFilters.GetOrCreateWeaponFilters().Filters.AttacksPerSecond = new StatFilterValue(doubleFilter); break;
            case GameType.PathOfExile2: searchFilters.GetOrCreateEquipmentFilters().Filters.AttacksPerSecond = new StatFilterValue(doubleFilter); break;
        }
    }
}
