using System.Text.Json;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;
using Sidekick.Data.Items;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public sealed class StatFilter : TradeFilter, INormalizableFilter
{
    public static AutoSelectPreferences GetDefault(GameType game)
    {
        var preferences = new AutoSelectPreferences
        {
            Mode = AutoSelectMode.Default,
            Rules =
            [
                new AutoSelectRule()
                {
                    Checked = true,
                    Conditions =
                    [
                        new AutoSelectCondition()
                        {
                            Type = AutoSelectConditionType.StatCategory,
                            Comparison = AutoSelectComparisonType.IsContainedIn,
                            Value = JsonSerializer.Serialize(new List<StatCategory>()
                            {
                                StatCategory.Fractured,
                            }, AutoSelectPreferences.JsonSerializerOptions),
                        },
                    ],
                },
            ],
        };

        if (game == GameType.PathOfExile2)
        {
            preferences.Rules.Add(new AutoSelectRule()
            {
                Checked = true,
                Conditions =
                [
                    new AutoSelectCondition()
                    {
                        Type = AutoSelectConditionType.Rarity,
                        Comparison = AutoSelectComparisonType.IsContainedIn,
                        Value = JsonSerializer.Serialize(new List<Rarity>()
                        {
                            Rarity.Normal,
                            Rarity.Magic,
                        }, AutoSelectPreferences.JsonSerializerOptions),
                    }
                ]
            });
        }

        return preferences;
    }

    public StatFilter(Stat stat, GameType game)
    {
        Stat = stat;
        Text = stat.Text;

        DefaultAutoSelect = GetDefault(game);
        UsePrimaryCategory = stat.Category is StatCategory.Fractured;
    }

    public override async Task<AutoSelectResult?> Initialize(Item item, ISettingsService settingsService)
    {
        var result = await base.Initialize(item, settingsService);
        if (result == null) return null;

        if (result.FillMinRange) Min = ((INormalizableFilter)this).NormalizeMinValue(result.NormalizeBy);
        if (result.FillMaxRange) Max = ((INormalizableFilter)this).NormalizeMaxValue(result.NormalizeBy);

        return result;
    }

    public Stat Stat { get; init; }

    public bool UsePrimaryCategory { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public double NormalizeValue => Stat.AverageValue;

    public bool NormalizeEnabled => true;

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked || Stat.Definitions.Count == 0)
        {
            return;
        }

        var stats = Stat.Definitions
            .SelectMany(definition => definition.TradeIds.Select(tradeId => new
            {
                Definition = definition,
                TradeId = tradeId,
            }))
            .DistinctBy(x => x.TradeId)
            .ToList();

        if (stats.Count > 1)
        {
            var categoryString = Stat.Category.GetValueAttribute();
            if (!UsePrimaryCategory && Stat.Category.HasExplicitStat())
            {
                categoryString = StatCategory.Explicit.GetValueAttribute();
            }

            if (stats.Any(x => x.TradeId.StartsWith(categoryString)))
            {
                stats = stats.Where(x => x.TradeId.StartsWith(categoryString)).ToList();
            }
        }

        if (stats.Count == 1)
        {
            query.GetOrCreateStatGroup(StatType.And).Filters.Add(new StatFilters()
            {
                Id = stats.First().TradeId,
                Value = new StatFilterValue(this),
            });
        }
        else
        {
            var countGroup = query.GetOrCreateStatGroup(StatType.Count);
            foreach (var stat in stats)
            {
                countGroup.Filters.Add(new StatFilters()
                {
                    Id = stat.TradeId,
                    Value = new StatFilterValue(this),
                });
            }

            if (countGroup.Value == null)
            {
                countGroup.Value = new StatFilterValue()
                {
                    Min = 0,
                };
            }

            if (stats.Count != 0)
            {
                countGroup.Value.Min += 1;
            }
        }
    }
}
