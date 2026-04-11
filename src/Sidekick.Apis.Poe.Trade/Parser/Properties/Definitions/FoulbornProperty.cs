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

public class FoulbornProperty(
    GameType game,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    private readonly ITradeFilterProvider tradeFilterProvider = serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override string Label => tradeFilterProvider.Foulborn?.Text ?? "Foulborn";

    public override void ParseAfterStats(Item item)
    {
        item.Properties.Foulborn = item.Stats.Any(x => x.Category == StatCategory.Mutated);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game == GameType.PathOfExile2 || item.Properties.Rarity != Rarity.Unique) return Task.FromResult<TradeFilter?>(null);

        var filter = new FoulbornFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(FoulbornProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class FoulbornFilter : TriStatePropertyFilter
{
    public FoulbornFilter()
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
                            Type = AutoSelectConditionType.Foulborn,
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
                            Type = AutoSelectConditionType.Foulborn,
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

        query.Filters.GetOrCreateMiscFilters().Filters.Foulborn = new SearchFilterOption(this);
    }
}
