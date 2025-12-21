using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class MirroredProperty(IServiceProvider serviceProvider) : PropertyDefinition
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
        if (TradeFilterProvicer.Mirrored == null) return Task.FromResult<TradeFilter?>(null);

        var filter = new MirroredFilter
        {
            Text = TradeFilterProvicer.Mirrored.Text ?? "Mirrored",
            Checked = null,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class MirroredFilter : TriStatePropertyFilter
{
    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (Checked == null)
        {
            return;
        }

        query.Filters.GetOrCreateMiscFilters().Filters.Mirrored = new SearchFilterOption(this);
    }
}
