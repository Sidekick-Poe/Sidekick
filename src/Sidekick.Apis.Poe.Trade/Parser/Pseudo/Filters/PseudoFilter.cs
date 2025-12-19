using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo.Filters;

public class PseudoFilter
{
    public required PseudoModifier Modifier { get; set; }

    public bool Checked { get; set; } = false;

    public double? Min { get; set; }

    public double? Max { get; set; }

    public void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked || string.IsNullOrEmpty(Modifier.ModifierId))
        {
            return;
        }

        query.GetOrCreateStatGroup(StatType.And).Filters.Add(new StatFilters()
        {
            Id = Modifier.ModifierId,
            Value = new StatFilterValue()
            {
                Min = Min,
                Max = Max,
            },
        });
    }
}
