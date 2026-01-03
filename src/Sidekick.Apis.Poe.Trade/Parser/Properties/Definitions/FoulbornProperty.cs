using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class FoulbornProperty(
    GameType game,
    ISettingsService settingsService,
    IGameLanguageProvider gameLanguageProvider,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    private readonly ITradeFilterProvider tradeFilterProvider = serviceProvider.GetRequiredService<ITradeFilterProvider>();

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

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        if (game == GameType.PathOfExile2) return null;
        if (tradeFilterProvider.Foulborn == null) return null;
        if (item.Properties.Rarity != Rarity.Unique) return null;

        var autoSelectKey = $"Trade_Filter_{nameof(FoulbornProperty)}_{game.GetValueAttribute()}";
        var filter = new FoulbornFilter
        {
            Text = tradeFilterProvider.Foulborn.Text ?? "Foulborn",
            AutoSelectSettingKey = autoSelectKey,
            AutoSelect = await settingsService.GetObject<AutoSelectPreferences>(autoSelectKey, () => null),
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
            Mode = AutoSelectMode.Conditionally,
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
                            Comparison = AutoSelectComparisonType.Equals,
                            Value = true.ToString(),
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
                            Comparison = AutoSelectComparisonType.Equals,
                            Value = false.ToString(),
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
