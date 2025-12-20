using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class FracturedProperty(IServiceProvider serviceProvider) : PropertyDefinition
{
    private ITradeFilterProvider TradeFilterProvicer => serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.WithStats,
    ];

    public override void Parse(Item item)
    {
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (TradeFilterProvicer.Fractured == null) return Task.FromResult<TradeFilter?>(null);

        var filter = new TriStatePropertyFilter(this)
        {
            Text = TradeFilterProvicer.Fractured.Text ?? "Fractured",
            Checked = null,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, TradeFilter filter)
    {
        if (filter is not TriStatePropertyFilter triStateFilter || triStateFilter.Checked == null)
        {
            return;
        }

        query.Filters.GetOrCreateMiscFilters().Filters.Fractured = new SearchFilterOption(triStateFilter);
    }
}
