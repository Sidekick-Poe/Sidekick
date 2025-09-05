namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;

public class ExpandablePropertiesFilter(ExpandablePropertiesDefinition definition, params BooleanPropertyFilter[] filters) : BooleanPropertyFilter(definition)
{
    public List<BooleanPropertyFilter> Filters { get; } = filters.ToList();
}
