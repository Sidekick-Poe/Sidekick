using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Results;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;

public class PropertyFilter
{
    public PropertyFilter(PropertyDefinition definition)
    {
        PrepareTradeRequest = (query, item) => definition.PrepareTradeRequest(query, item, this);
    }

    protected PropertyFilter(Action<Query, Item> prepareTradeRequest)
    {
        PrepareTradeRequest = prepareTradeRequest;
    }
    
    public bool ShowRow { get; init; } = true;

    public bool Checked { get; set; }

    public required string Text { get; init; }

    public string? Hint { get; init; }

    public LineContentType Type { get; init; } = LineContentType.Simple;
    
    public Action<Query, Item> PrepareTradeRequest { get; set; }
}
