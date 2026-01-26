using System.Text.Json;
using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ItemLevelProperty(
    GameType game,
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

    public override string Label => gameLanguageProvider.Language.DescriptionItemLevel;

    public override void Parse(Item item)
    {
        item.Properties.ItemLevel = GetInt(Pattern, item.Text);
    }

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.ItemLevel <= 0) return null;

        var filter = new ItemLevelFilter(game)
        {
            Text = Label,
            Value = item.Properties.ItemLevel,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(ItemLevelProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return filter;
    }
}

public class ItemLevelFilter : IntPropertyFilter
{
    public ItemLevelFilter(GameType game)
    {
        Game = game;
        if (game == GameType.PathOfExile1)
        {
            DefaultAutoSelect = new AutoSelectPreferences()
            {
                Mode = AutoSelectMode.Default,
                Rules =
                [
                    new()
                    {
                        Checked = true,
                        NormalizeBy = 0,
                        FillMinRange = true,
                        FillMaxRange = false,
                        Conditions =
                        [
                            new()
                            {
                                Type = AutoSelectConditionType.ItemLevel,
                                Comparison = AutoSelectComparisonType.GreaterThanOrEqual,
                                Value = 80.ToString(),
                            },
                            new()
                            {
                                Type = AutoSelectConditionType.MapTier,
                                Comparison = AutoSelectComparisonType.Equals,
                                Value = 0.ToString(),
                            },
                            new()
                            {
                                Type = AutoSelectConditionType.Rarity,
                                Comparison = AutoSelectComparisonType.IsContainedIn,
                                Value = JsonSerializer.Serialize(new List<Rarity>()
                                {
                                    Rarity.Normal,
                                    Rarity.Magic,
                                    Rarity.Rare,
                                }, AutoSelectPreferences.JsonSerializerOptions),
                            },
                        ],
                    },
                ],
            };
        }
        else
        {
            DefaultAutoSelect = new AutoSelectPreferences()
            {
                Mode = AutoSelectMode.Default,
                Rules =
                [
                    new()
                    {
                        Checked = true,
                        NormalizeBy = 0,
                        FillMinRange = true,
                        FillMaxRange = false,
                        Conditions =
                        [
                            new()
                            {
                                Type = AutoSelectConditionType.ItemLevel,
                                Comparison = AutoSelectComparisonType.GreaterThanOrEqual,
                                Value = 82.ToString(),
                            },
                            new()
                            {
                                Type = AutoSelectConditionType.MapTier,
                                Comparison = AutoSelectComparisonType.Equals,
                                Value = 0.ToString(),
                            },
                            new()
                            {
                                Type = AutoSelectConditionType.Rarity,
                                Comparison = AutoSelectComparisonType.IsContainedIn,
                                Value = JsonSerializer.Serialize(new List<Rarity>()
                                {
                                    Rarity.Normal,
                                    Rarity.Magic,
                                    Rarity.Rare,
                                }, AutoSelectPreferences.JsonSerializerOptions),
                            },
                        ],
                    },
                ],
            };
        }
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
