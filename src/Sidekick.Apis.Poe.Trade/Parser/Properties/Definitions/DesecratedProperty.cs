using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class DesecratedProperty(IServiceProvider serviceProvider, GameType game) : PropertyDefinition
{
    private ITradeFilterProvider TradeFilterProvicer => serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override List<ItemClass> ValidItemClasses { get; } = [
        ..ItemClassConstants.Equipment,
        ..ItemClassConstants.Accessories,
        ..ItemClassConstants.Weapons,
    ];

    public override void Parse(Item item)
    {
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game == GameType.PathOfExile1) return Task.FromResult<TradeFilter?>(null);
        if (TradeFilterProvicer.Desecrated == null) return Task.FromResult<TradeFilter?>(null);

        var filter = new TriStatePropertyFilter(this)
        {
            Text = TradeFilterProvicer.Desecrated.Text ?? "Desecrated",
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

        query.Filters.GetOrCreateMiscFilters().Filters.Desecrated = new SearchFilterOption(triStateFilter);
    }
}
