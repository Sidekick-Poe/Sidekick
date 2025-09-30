namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;

public class ItemClassPropertyFilter : PropertyFilter
{
    internal ItemClassPropertyFilter(PropertyDefinition definition)
        : base(definition)
    {
    }

    public required string ItemClass { get; init; }
    public required string BaseType { get; init; }
}
