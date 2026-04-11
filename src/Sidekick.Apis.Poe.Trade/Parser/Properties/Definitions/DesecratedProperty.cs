using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Items;
using Sidekick.Data.Stats;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class DesecratedProperty(
    GameType game,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    private readonly ITradeFilterProvider tradeFilterProvider = serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override string Label => tradeFilterProvider.Desecrated?.Text ?? "Desecrated";

    public override void Parse(Item item)
    {
        item.Properties.Desecrated = item.Stats.Any(x => x.Category == StatCategory.Desecrated);
    }

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
        DefaultAutoSelect = new AutoSelectPreferences()
        {
            Mode = AutoSelectMode.Default,
            Rules =
            [
                new()
                {
                    Checked = true,
                    Conditions =
                    [
                        new()
                        {
                            Type = AutoSelectConditionType.Desecrated,
                            Comparison = AutoSelectComparisonType.True,
                        },
                    ],
                },
                new()
                {
                    Checked = false,
                    Conditions =
                    [
                        new()
                        {
                            Type = AutoSelectConditionType.Desecrated,
                            Comparison = AutoSelectComparisonType.False,
                        },
                    ],
                },
            ],
        };
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
