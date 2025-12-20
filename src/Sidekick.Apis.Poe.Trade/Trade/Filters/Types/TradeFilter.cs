using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class TradeFilter
{
    public TradeFilter(PropertyDefinition definition)
    {
        PrepareTradeRequest = (query, item) => definition.PrepareTradeRequest(query, item, this);
    }

    protected TradeFilter()
    {
    }

    public bool ShowRow { get; init; } = true;

    public virtual bool Checked { get; set; }

    public string Text { get; init; } = string.Empty;

    public string? Hint { get; init; }

    public LineContentType Type { get; init; } = LineContentType.Simple;

    public Action<Query, Item>? PrepareTradeRequest { get; set; }
}
