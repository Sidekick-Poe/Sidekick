using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class StringPropertyFilter : TradeFilter
{
    public required string Value { get; init; }
}
