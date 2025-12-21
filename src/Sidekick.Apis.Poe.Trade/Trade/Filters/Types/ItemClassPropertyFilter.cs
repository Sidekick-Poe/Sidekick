using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class ItemClassPropertyFilter : TradeFilter
{
    public required string ItemClass { get; init; }
    public required string BaseType { get; init; }
}
