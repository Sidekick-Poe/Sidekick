using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties;

public class CollapsiblePropertiesDefinition
(
    string? label,
    params PropertyDefinition[] definitions
) : PropertyDefinition
{
    public List<PropertyDefinition> Definitions { get; } = definitions.ToList();

    public override List<Category> ValidCategories => Definitions.SelectMany(x => x.ValidCategories).Distinct().ToList();

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        var filter = new CollapsiblePropertiesFilter(this)
        {
            Text = label ?? string.Empty,
        };

        filter.Filters.AddRange(Definitions.Select(x => x.GetFilter(item, normalizeValue, filterType)).Where(x => x != null)!);

        return filter.Filters.Count == 0 ? null : filter;
    }

    public override void PrepareTradeRequest(Query query, Item item, BooleanPropertyFilter filter)
    {
        if (filter is not CollapsiblePropertiesFilter collapsiblePropertiesFilter) return;

        foreach (var childFilter in collapsiblePropertiesFilter.Filters)
        {
            childFilter.Definition.PrepareTradeRequest(query, item, childFilter);
        }
    }
}
