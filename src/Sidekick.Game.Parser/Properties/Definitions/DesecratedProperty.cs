using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Common.Enums;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Stats;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using Sidekick.Game.Providers;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class DesecratedProperty(
    GameType game,
    TradeFilterProvider tradeFilterProvider) : PropertyDefinition
{
    public override string Label => tradeFilterProvider.Desecrated?.Text ?? "Desecrated";

    public override void ParseAfterStats(Item item)
    {
        item.Properties.Desecrated = item.Stats.Any(x => x.Category == StatCategory.Desecrated);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game == GameType.Poe1) return Task.FromResult<TradeFilter?>(null);
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
