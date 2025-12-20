using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties;

public class ExpandablePropertiesDefinition
(
    string? label,
    params PropertyDefinition[] definitions
) : PropertyDefinition
{
    private List<PropertyDefinition> Definitions { get; } = definitions.ToList();

    public override List<ItemClass> ValidItemClasses => Definitions.SelectMany(x => x.ValidItemClasses).Distinct().ToList();

    public override void Parse(Item item)
    {
        foreach (var definition in Definitions)
        {
            definition.Parse(item);
        }
    }

    public override void ParseAfterStats(Item item)
    {
        foreach (var definition in Definitions)
        {
            definition.ParseAfterStats(item);
        }
    }

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        var filter = new ExpandableFilter(this)
        {
            Text = label ?? string.Empty,
        };

        foreach (var definition in Definitions)
        {
            var definitionFilter = await definition.GetFilter(item);
            if (definitionFilter != null)
            {
                filter.Filters.Add(definitionFilter);
            }
        }

        if (filter.Filters.Count == 0) return null;

        return filter;
    }

    public override List<TradeFilter>? GetFilters(Item item)
    {
        var filter = new ExpandableFilter(this)
        {
            Text = label ?? string.Empty,
        };

        foreach (var definition in Definitions)
        {
            var definitionFilters = definition.GetFilters(item);
            if (definitionFilters != null)
            {
                filter.Filters.AddRange(definitionFilters);
            }
        }

        return filter.Filters.Count == 0 ? null : [filter];
    }

    public override void PrepareTradeRequest(Query query, Item item, TradeFilter filter)
    {
        if (filter is not ExpandableFilter expandablePropertiesFilter) return;

        foreach (var childFilter in expandablePropertiesFilter.Filters)
        {
            if (childFilter.PrepareTradeRequest == null) continue;
            childFilter.PrepareTradeRequest(query, item);
        }
    }
}
