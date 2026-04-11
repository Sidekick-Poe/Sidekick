using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Common.Settings;
using Sidekick.Data.Items;
namespace Sidekick.Apis.Poe.Trade.Filters.Types;

public sealed class ExpandableFilter : TradeFilter
{
    public ExpandableFilter(string text, bool checkedByDefault, params TradeFilter[] filters)
    {
        Text = text;
        CheckedByDefault = checkedByDefault;
        Checked = checkedByDefault;
        Filters = filters.ToList();
    }

    private bool CheckedByDefault { get; }

    public List<TradeFilter> Filters { get; }

    public override async Task<AutoSelectResult?> Initialize(Item item, ISettingsService settingsService)
    {
        foreach (var filter in Filters)
        {
            await filter.Initialize(item, settingsService);
        }

        var result = await base.Initialize(item, settingsService);
        Checked |= CheckedByDefault;
        return result;
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        foreach (var filter in Filters)
        {
            filter.PrepareTradeRequest(query, item);
        }
    }
}
