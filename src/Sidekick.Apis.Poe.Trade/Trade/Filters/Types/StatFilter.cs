using System.Text.Json;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Settings;
using Sidekick.Data.Items.Models;
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
        if (!Checked || Stat.MatchedPatterns.Count == 0)
        {
            return;
        }

        var stats = Stat.MatchedPatterns.ToList();
        if (stats.Count > 1)
        {
            if (UsePrimaryCategory)
            {
                stats = stats.Where(x => x.Category == Stat.Category).ToList();
            }
            else if (stats.Any(x => x.Category == StatCategory.Explicit))
            {
                stats = stats.Where(x => x.Category == StatCategory.Explicit).ToList();
            }
        }

        var tradeIds = stats.SelectMany(x => x.TradeIds).ToList();
        if (tradeIds.Count == 1)
        {
            query.GetOrCreateStatGroup(StatType.And).Filters.Add(new StatFilters()
            {
                Id = tradeIds.First(),
                Value = new StatFilterValue(this),
            });
        }
        else
        {
            var countGroup = query.GetOrCreateStatGroup(StatType.Count);
            foreach (var tradeId in tradeIds)
            {
                countGroup.Filters.Add(new StatFilters()
                {
                    Id = tradeId,
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
