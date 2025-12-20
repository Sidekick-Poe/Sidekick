using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class FoulbornProperty(IServiceProvider serviceProvider, GameType game) : PropertyDefinition
{
    private ITradeFilterProvider TradeFilterProvicer => serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override List<ItemClass> ValidItemClasses { get; } = [
        ..ItemClassConstants.Equipment,
        ..ItemClassConstants.Weapons,
        ..ItemClassConstants.Accessories,
        ..ItemClassConstants.Jewels,
        ..ItemClassConstants.Flasks,
    ];

    public override void ParseAfterStats(Item item)
    {
        if (game == GameType.PathOfExile2) return;
        if (item.Properties.Rarity != Rarity.Unique) return;

        item.Properties.Foulborn = item.Stats.Any(x => x.ApiInformation.Any(y => y.Category == StatCategory.Mutated));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game == GameType.PathOfExile2) return Task.FromResult<TradeFilter?>(null);
        if (TradeFilterProvicer.Foulborn == null) return Task.FromResult<TradeFilter?>(null);
        if (item.Properties.Rarity != Rarity.Unique) return Task.FromResult<TradeFilter?>(null);

        var filter = new TriStatePropertyFilter(this)
        {
            Text = TradeFilterProvicer.Foulborn.Text ?? "Foulborn",
            Checked = item.Properties.Foulborn,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, TradeFilter filter)
    {
        if (filter is not TriStatePropertyFilter triStateFilter || triStateFilter.Checked == null)
        {
            return;
        }

        query.Filters.GetOrCreateMiscFilters().Filters.Foulborn = new SearchFilterOption(triStateFilter);
    }
}
