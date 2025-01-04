using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class AreaLevelProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex? Pattern { get; set; }

    public override List<Category> ValidCategories { get; } = [Category.Sanctum, Category.Logbook, Category.Contract];

    public override void Initialize()
    {
        Pattern = gameLanguageProvider.Language.DescriptionAreaLevel.ToRegexIntCapture();
    }

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.AreaLevel = GetInt(Pattern, propertyBlock);
        if (itemProperties.AreaLevel > 0) propertyBlock.Parsed = true;
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue)
    {
        if (item.Properties.AreaLevel <= 0) return null;

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionAreaLevel,
            NormalizeEnabled = false,
            NormalizeValue = normalizeValue,
            Value = item.Properties.AreaLevel,
            Checked = true,
        };
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        searchFilters.GetOrCreateMapFilters().Filters.AreaLevel = intFilter.Checked ? new StatFilterValue(intFilter) : null;
    }
}
