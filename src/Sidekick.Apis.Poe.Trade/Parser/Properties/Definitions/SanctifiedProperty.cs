using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class SanctifiedProperty(IServiceProvider serviceProvider, GameType game) : PropertyDefinition
{
    private ITradeFilterProvider TradeFilterProvicer => serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Equipment,
        ..ItemClassConstants.Weapons,
        ..ItemClassConstants.Accessories,
    ];

    public override void Parse(Item item)
    {
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game == GameType.PathOfExile1) return Task.FromResult<TradeFilter?>(null);
        if (TradeFilterProvicer.Sanctified == null) return Task.FromResult<TradeFilter?>(null);

        var filter = new SanctifiedFilter
        {
            Text = TradeFilterProvicer.Sanctified.Text ?? "Sanctified",
            Checked = null,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class SanctifiedFilter : TriStatePropertyFilter
{
    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (Checked == null)
        {
            return;
        }

        query.Filters.GetOrCreateMiscFilters().Filters.Sanctified = new SearchFilterOption(this);
    }
}
