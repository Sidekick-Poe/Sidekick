using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Results;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class MonsterPackSizeProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionMonsterPackSize.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionMonsterPackSize.ToRegexIsAugmented();

    public override List<Category> ValidCategories { get; } = [Category.Map, Category.Contract, Category.Logbook];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.MonsterPackSize = GetInt(Pattern, propertyBlock);
        if (itemProperties.MonsterPackSize == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) itemProperties.AugmentedProperties.Add(nameof(ItemProperties.MonsterPackSize));
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.MonsterPackSize <= 0) return null;

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionMonsterPackSize,
            NormalizeEnabled = true,
            NormalizeValue = normalizeValue,
            Value = item.Properties.MonsterPackSize,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.MonsterPackSize)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        filter.ChangeFilterType(filterType);
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        searchFilters.GetOrCreateMapFilters().Filters.MonsterPackSize = new StatFilterValue(intFilter);
    }
}
