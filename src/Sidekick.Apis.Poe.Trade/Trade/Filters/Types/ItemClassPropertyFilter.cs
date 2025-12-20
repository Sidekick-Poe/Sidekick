using Sidekick.Apis.Poe.Trade.Parser.Properties;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class ItemClassPropertyFilter : TradeFilter
{
    internal ItemClassPropertyFilter(PropertyDefinition definition)
        : base(definition)
    {
    }

    public required string ItemClass { get; init; }
    public required string BaseType { get; init; }
}
