using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Results;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class ArmourProperty
(
    IGameLanguageProvider gameLanguageProvider,
    GameType game
) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionArmour.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionArmour.ToRegexIsAugmented();

    public override List<Category> ValidCategories { get; } = [Category.Armour];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.Armour = GetInt(Pattern, propertyBlock);
        if (itemProperties.Armour == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) itemProperties.AugmentedProperties.Add(nameof(ItemProperties.Armour));
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.Armour <= 0) return null;

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionArmour,
            NormalizeEnabled = true,
            NormalizeValue = normalizeValue,
            Value = item.Properties.ArmourWithQuality,
            OriginalValue = item.Properties.Armour,
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.Armour)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        filter.ChangeFilterType(filterType);
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        switch (game)
        {
            case GameType.PathOfExile: searchFilters.GetOrCreateArmourFilters().Filters.Armour = new StatFilterValue(intFilter); break;
            case GameType.PathOfExile2: searchFilters.GetOrCreateEquipmentFilters().Filters.Armour = new StatFilterValue(intFilter); break;
        }
    }
}
