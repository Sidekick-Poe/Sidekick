using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;

public class PseudoFilter
{
    public required PseudoStat Stat { get; set; }

    public bool Checked { get; set; } = false;

    public double? Min { get; set; }

    public double? Max { get; set; }

    public void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked || string.IsNullOrEmpty(Stat.Id))
        {
            return;
        }

        query.GetOrCreateStatGroup(StatType.And).Filters.Add(new StatFilters()
        {
            Id = Stat.Id,
            Value = new StatFilterValue()
            {
                Min = Min,
                Max = Max,
            },
        });
    }
}
