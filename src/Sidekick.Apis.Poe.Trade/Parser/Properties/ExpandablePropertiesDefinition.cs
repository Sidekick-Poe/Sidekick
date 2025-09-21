using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties;

public class ExpandablePropertiesDefinition
(
    string? label,
    params PropertyDefinition[] definitions
) : PropertyDefinition
{
    public List<PropertyDefinition> Definitions { get; } = definitions.ToList();

    public override List<Category> ValidCategories => Definitions.SelectMany(x => x.ValidCategories).Distinct().ToList();

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem, ItemHeader header)
    {
        foreach (var definition in Definitions)
        {
            definition.Parse(itemProperties, parsingItem, header);
        }
    }

    public override void ParseAfterModifiers(Item item, ParsingItem parsingItem)
    {
        foreach (var definition in Definitions)
        {
            definition.ParseAfterModifiers(item, parsingItem);
        }
    }

    public override async Task<PropertyFilter?> GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        var filter = new ExpandablePropertiesFilter(this)
        {
            Text = label ?? string.Empty,
        };

        foreach (var definition in Definitions)
        {
            var definitionFilter = await definition.GetFilter(item, normalizeValue, filterType);
            if (definitionFilter != null)
            {
                filter.Filters.Add(definitionFilter);
            }
        }

        if (filter.Filters.Count == 0) return null;

        return filter;
    }

    public override List<PropertyFilter>? GetFilters(Item item, double normalizeValue, FilterType filterType)
    {
        var filter = new ExpandablePropertiesFilter(this)
        {
            Text = label ?? string.Empty,
        };

        foreach (var definition in Definitions)
        {
            var definitionFilters = definition.GetFilters(item, normalizeValue, filterType);
            if (definitionFilters != null)
            {
                filter.Filters.AddRange(definitionFilters);
            }
        }

        return filter.Filters.Count == 0 ? null : [filter];
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (filter is not ExpandablePropertiesFilter expandablePropertiesFilter) return;

        foreach (var childFilter in expandablePropertiesFilter.Filters)
        {
            childFilter.Definition.PrepareTradeRequest(query, item, childFilter);
        }
    }
}
