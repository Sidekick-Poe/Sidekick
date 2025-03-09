using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class MapTierProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionMapTier.ToRegexIntCapture();

    public override List<Category> ValidCategories { get; } = [Category.Map];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.MapTier = GetInt(Pattern, propertyBlock);
        if (itemProperties.MapTier > 0) propertyBlock.Parsed = true;
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue)
    {
        if (item.Properties.MapTier <= 0) return null;

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionMapTier,
            NormalizeEnabled = false,
            NormalizeValue = normalizeValue,
            Value = item.Properties.MapTier,
            Checked = true,
        };
        filter.NormalizeMinValue();
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        searchFilters.MapFilters.Filters.MapTier = new StatFilterValue(intFilter);
    }
}
