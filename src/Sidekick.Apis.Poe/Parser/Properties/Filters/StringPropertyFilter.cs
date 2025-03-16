using Sidekick.Apis.Poe.Trade.Models;

namespace Sidekick.Apis.Poe.Parser.Properties.Filters;

public class StringPropertyFilter : BooleanPropertyFilter
{
    internal StringPropertyFilter(PropertyDefinition definition)
        : base(definition)
    {
    }

    public required string Value { get; init; }

    public required LineContentType Type { get; init; }
}
