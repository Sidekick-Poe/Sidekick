using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class FracturedProperty(
    GameType game,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    private readonly ITradeFilterProvider tradeFilterProvider = serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override string Label => tradeFilterProvider.Fractured?.Text ?? "Fractured";

    public override void Parse(Item item) {}

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (tradeFilterProvider.Fractured == null) return Task.FromResult<TradeFilter?>(null);

        var filter = new FracturedFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(FracturedProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class FracturedFilter : TriStatePropertyFilter
{
    public FracturedFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(null);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (Checked == null)
        {
            return;
        }

        query.Filters.GetOrCreateMiscFilters().Filters.Fractured = new SearchFilterOption(this);
    }
}
