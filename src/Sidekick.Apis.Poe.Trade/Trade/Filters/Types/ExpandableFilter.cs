using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class ExpandableFilter : TradeFilter
{
    public ExpandableFilter(string text, params TradeFilter[] filters)
    {
        Text = text;
        Filters = filters.ToList();
    }

    public List<TradeFilter> Filters { get; }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        foreach (var filter in Filters)
        {
            filter.PrepareTradeRequest(query, item);
        }
    }
}
