using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class ExpandableFilter : TradeFilter
{
    public ExpandableFilter(string text, params TradeFilter[] filters)
    {
        Text = text;
        Filters = filters.ToList();
    }

    public List<TradeFilter> Filters { get; }

    public override async Task<AutoSelectResult?> Initialize(Item item, ISettingsService settingsService)
    {
        foreach (var filter in Filters)
        {
            await filter.Initialize(item, settingsService);
        }

        return await base.Initialize(item, settingsService);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        foreach (var filter in Filters)
        {
            filter.PrepareTradeRequest(query, item);
        }
    }
}
