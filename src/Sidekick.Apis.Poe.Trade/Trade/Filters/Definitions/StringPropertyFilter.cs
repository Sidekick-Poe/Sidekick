using Sidekick.Apis.Poe.Trade.Parser.Properties;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;

public class StringPropertyFilter : TradeFilter
{
    internal StringPropertyFilter(PropertyDefinition definition)
        : base(definition)
    {
    }

    public required string Value { get; init; }
}
