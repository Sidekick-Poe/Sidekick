using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class DesecratedProperty(
    GameType game,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    private readonly ITradeFilterProvider tradeFilterProvider = serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Equipment,
        ..ItemClassConstants.Accessories,
        ..ItemClassConstants.Weapons,
    ];

    public override string Label => tradeFilterProvider.Desecrated?.Text ?? "Desecrated";

    public override void Parse(Item item) {}

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game == GameType.PathOfExile1) return Task.FromResult<TradeFilter?>(null);
        if (tradeFilterProvider.Desecrated == null) return Task.FromResult<TradeFilter?>(null);

        var filter = new DesecratedFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(DesecratedProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class DesecratedFilter : TriStatePropertyFilter
{
    public DesecratedFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(null);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (Checked == null)
        {
            return;
        }

        query.Filters.GetOrCreateMiscFilters().Filters.Desecrated = new SearchFilterOption(this);
    }
}
