using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class MapTierProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionMapTier.ToRegexIntCapture();

    public override List<Category> ValidItemClasses { get; } = [Category.Map];

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.MapTier = GetInt(Pattern, propertyBlock);
        if (item.Properties.MapTier > 0) propertyBlock.Parsed = true;
    }

    public override Task<PropertyFilter?> GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.MapTier <= 0) return Task.FromResult<PropertyFilter?>(null);

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionMapTier,
            NormalizeEnabled = false,
            NormalizeValue = normalizeValue,
            Value = item.Properties.MapTier,
            Checked = true,
        };
        filter.ChangeFilterType(filterType);
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        query.Filters.GetOrCreateMapFilters().Filters.MapTier = new StatFilterValue(intFilter);
    }
}
