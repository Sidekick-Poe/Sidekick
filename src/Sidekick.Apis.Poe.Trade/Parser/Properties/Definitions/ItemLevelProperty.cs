using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ItemLevelProperty(
    GameType game,
    ISettingsService settingsService,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionItemLevel.ToRegexIntCapture();

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Equipment,
        ..ItemClassConstants.Weapons,
        ..ItemClassConstants.Accessories,

        ..ItemClassConstants.Flasks,
        ..ItemClassConstants.Jewels,
        ..ItemClassConstants.Areas,

        ItemClass.SanctumRelic,
        ItemClass.SanctumResearch,

        ItemClass.Wombgift,
        ItemClass.Graft,
    ];

    public override void Parse(Item item)
    {
        item.Properties.ItemLevel = GetInt(Pattern, item.Text);
    }

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.ItemLevel <= 0) return null;

        var autoSelectKey = $"Trade_Filter_{nameof(ItemLevelProperty)}_{game.GetValueAttribute()}";
        var filter = new ItemLevelFilter(game)
        {
            Text = gameLanguageProvider.Language.DescriptionItemLevel,
            NormalizeEnabled = false,
            Value = item.Properties.ItemLevel,
            AutoSelectSettingKey = autoSelectKey,
            AutoSelect = await settingsService.GetObject<AutoSelectPreferences>(autoSelectKey, () => null),
        };
        return filter;
    }
}

public class ItemLevelFilter : IntPropertyFilter
{
    public ItemLevelFilter(GameType game)
    {
        Game = game;
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
                            Type = AutoSelectConditionType.Equals,
                            Value = GameType.PathOfExile1,
                            Expression = x => x.Game,
                        },
                        new()
                        {
                            Type = AutoSelectConditionType.GreaterThanOrEqual,
                            Value = 80,
                            Expression = x => x.Properties.ItemLevel,
                        },
                        new()
                        {
                            Type = AutoSelectConditionType.Equals,
                            Value = 0,
                            Expression = x => x.Properties.MapTier,
                        },
                        new()
                        {
                            Type = AutoSelectConditionType.LesserThan,
                            Value = (int)Rarity.Unique,
                            Expression = x => (int)x.Properties.Rarity,
                        },
                    ],
                },
            ],
        };
    }

    private GameType Game { get; }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        switch (Game)
        {
            case GameType.PathOfExile1: query.Filters.GetOrCreateMiscFilters().Filters.ItemLevel = new StatFilterValue(this); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateTypeFilters().Filters.ItemLevel = new StatFilterValue(this); break;
        }
    }
}
