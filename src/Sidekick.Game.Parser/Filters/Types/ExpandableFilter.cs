using Sidekick.Common.Settings;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
namespace Sidekick.Game.Parser.Filters.Types;

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
