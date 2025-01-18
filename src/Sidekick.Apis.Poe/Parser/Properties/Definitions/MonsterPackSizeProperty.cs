using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class MonsterPackSizeProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex? Pattern { get; set; }

    public override List<Category> ValidCategories { get; } = [Category.Map, Category.Contract];

    public override void Initialize()
    {
        Pattern = gameLanguageProvider.Language.DescriptionMonsterPackSize.ToRegexIntCapture();
    }

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.MonsterPackSize = GetInt(Pattern, propertyBlock);
        if (itemProperties.MonsterPackSize > 0) propertyBlock.Parsed = true;
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue)
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
        };
        filter.NormalizeMinValue();
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        searchFilters.GetOrCreateMapFilters().Filters.MonsterPackSize = new StatFilterValue(intFilter);
    }
}
