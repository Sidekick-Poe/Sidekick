using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;

public class TradeFilter
{
    public TradeFilter(PropertyDefinition definition)
    {
        PrepareTradeRequest = (query, item) => definition.PrepareTradeRequest(query, item, this);
    }

    public bool ShowRow { get; init; } = true;

    public bool Checked { get; set; }

    public required string Text { get; init; }

    public string? Hint { get; init; }

    public LineContentType Type { get; init; } = LineContentType.Simple;

    public Action<Query, Item> PrepareTradeRequest { get; set; }
}
