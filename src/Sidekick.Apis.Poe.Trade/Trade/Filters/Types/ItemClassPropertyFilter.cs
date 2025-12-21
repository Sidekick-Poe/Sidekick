namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class ItemClassPropertyFilter : TradeFilter
{
    public required string ItemClass { get; init; }
    public required string BaseType { get; init; }
}
