namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;

public class StringPropertyFilter : PropertyFilter
{
    internal StringPropertyFilter(PropertyDefinition definition)
        : base(definition)
    {
    }

    public required string Value { get; init; }
}
