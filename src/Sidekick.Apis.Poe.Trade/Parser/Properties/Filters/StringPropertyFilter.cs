namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;

public class StringPropertyFilter : BooleanPropertyFilter
{
    internal StringPropertyFilter(PropertyDefinition definition)
        : base(definition)
    {
    }

    public required string Value { get; init; }
}
