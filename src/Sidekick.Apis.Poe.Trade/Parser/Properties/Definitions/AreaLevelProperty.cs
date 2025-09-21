using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class AreaLevelProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionAreaLevel.ToRegexIntCapture();

    public override List<Category> ValidCategories { get; } = [Category.Sanctum, Category.Logbook, Category.Contract, Category.Map];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem, ItemHeader header)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.AreaLevel = GetInt(Pattern, propertyBlock);
        if (itemProperties.AreaLevel > 0) propertyBlock.Parsed = true;
    }

    public override Task<PropertyFilter?> GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.AreaLevel <= 0) return Task.FromResult<PropertyFilter?>(null);

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionAreaLevel,
            NormalizeEnabled = false,
            NormalizeValue = normalizeValue,
            Value = item.Properties.AreaLevel,
            Checked = true,
        };
        filter.ChangeFilterType(filterType);
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        query.Filters.GetOrCreateMapFilters().Filters.AreaLevel = intFilter.Checked ? new StatFilterValue(intFilter) : null;
    }
}
