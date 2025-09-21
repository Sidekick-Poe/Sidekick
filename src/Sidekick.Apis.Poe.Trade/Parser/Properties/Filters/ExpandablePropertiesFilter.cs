namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;

public class ExpandablePropertiesFilter(ExpandablePropertiesDefinition definition, params PropertyFilter[] filters) : PropertyFilter(definition)
{
    public List<PropertyFilter> Filters { get; } = filters.ToList();
}
