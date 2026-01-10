using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ExpandableProperty
(
    string? label,
    params PropertyDefinition[] definitions
) : PropertyDefinition
{
    public List<PropertyDefinition> Definitions { get; } = definitions.ToList();

    public override List<ItemClass> ValidItemClasses => Definitions.SelectMany(x => x.ValidItemClasses).Distinct().ToList();

    public override string Label => label ?? string.Empty;

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
        List<TradeFilter> filters = [];
        foreach (var definition in Definitions)
        {
            var definitionFilter = await definition.GetFilter(item);
            if (definitionFilter != null)
            {
                filters.Add(definitionFilter);
            }
        }

        if (filters.Count == 0) return null;

        return new ExpandableFilter(Label, filters.ToArray());
    }
}
