using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class PseudoFilter : TradeFilter
{
    public PseudoFilter(PseudoStat stat)
    {
        Stat = stat;
        Text = stat.Text;
    }

    public PseudoStat Stat { get; init; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public override void PrepareTradeRequest(Query query, Item item)
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
