using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class FoulbornProperty(
    GameType game,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    private readonly ITradeFilterProvider tradeFilterProvider = serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Equipment,
        ..ItemClassConstants.Weapons,
        ..ItemClassConstants.Accessories,
        ..ItemClassConstants.Jewels,
        ..ItemClassConstants.Flasks,
    ];

    public override string Label => tradeFilterProvider.Foulborn?.Text ?? "Foulborn";

    public override void ParseAfterStats(Item item)
    {
        if (game == GameType.PathOfExile2) return;
        if (item.Properties.Rarity != Rarity.Unique) return;

        item.Properties.Foulborn = item.Stats.Any(x => x.ApiInformation.Any(y => y.Category == StatCategory.Mutated));
    }

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        if (game == GameType.PathOfExile2) return null;
        if (tradeFilterProvider.Foulborn == null) return null;
        if (item.Properties.Rarity != Rarity.Unique) return null;

        var filter = new FoulbornFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(FoulbornProperty)}_{game.GetValueAttribute()}",
        };
        return filter;
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
