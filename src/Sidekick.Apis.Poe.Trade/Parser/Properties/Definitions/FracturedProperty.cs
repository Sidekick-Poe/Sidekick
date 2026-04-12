using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.ItemClasses;
using Sidekick.Data.Items;
using Sidekick.Data.Stats;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class FracturedProperty(
    GameType game,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    private readonly ITradeFilterProvider tradeFilterProvider = serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override string Label => tradeFilterProvider.Fractured?.Text ?? "Fractured";

    public override void ParseAfterStats(Item item)
    {
        item.Properties.Fractured = item.Stats.Any(x => x.Category == StatCategory.Fractured);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (!item.ItemClass.CanHaveStats() || item.ItemClass.Type == ItemClass.ActiveSkillGem || item.Properties.Rarity == Rarity.Unique) return Task.FromResult<TradeFilter?>(null);

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
                            Type = AutoSelectConditionType.Fractured,
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
                            Type = AutoSelectConditionType.Fractured,
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

        query.Filters.GetOrCreateMiscFilters().Filters.Fractured = new SearchFilterOption(this);
    }
}
