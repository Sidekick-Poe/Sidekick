namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;

public class CollapsiblePropertiesFilter(CollapsiblePropertiesDefinition definition, params BooleanPropertyFilter[] filters) : BooleanPropertyFilter(definition)
{
    public List<BooleanPropertyFilter> Filters { get; } = filters.ToList();
}
