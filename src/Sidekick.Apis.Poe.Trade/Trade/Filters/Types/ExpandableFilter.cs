using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class ExpandableFilter : TradeFilter
{
    public ExpandableFilter(string text, params TradeFilter[] filters)
    {
        Text = text;
        Filters = filters.ToList();
        PrepareTradeRequest = PrepareChildFilters;
    }

    public List<TradeFilter> Filters { get; }

    private void PrepareChildFilters(Query query, Item item)
    {
        foreach (var filter in Filters)
        {
            if (filter.PrepareTradeRequest == null) continue;
            filter.PrepareTradeRequest(query, item);
        }
    }
}
