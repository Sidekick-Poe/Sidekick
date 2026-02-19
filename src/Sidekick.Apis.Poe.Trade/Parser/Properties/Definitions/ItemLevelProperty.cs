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
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionItemLevel.ToRegexIntCapture();

    public override string Label => currentGameLanguage.Language.DescriptionItemLevel;

    public override void Parse(Item item)
    {
        if (!ItemClassConstants.Equipment.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Weapons.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Accessories.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Flasks.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Jewels.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Areas.Contains(item.Properties.ItemClass) &&
            item.Properties.ItemClass != ItemClass.SanctumRelic &&
            item.Properties.ItemClass != ItemClass.SanctumResearch &&
            item.Properties.ItemClass != ItemClass.Wombgift &&
            item.Properties.ItemClass != ItemClass.Graft) return;

        item.Properties.ItemLevel = GetInt(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.ItemLevel <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new ItemLevelFilter(game)
        {
            Text = Label,
            Value = item.Properties.ItemLevel,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(ItemLevelProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
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
