using System.Text.Json;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Settings;
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

        var categories = stat.ApiInformation.Select(x => x.Category).Distinct().ToList();
        if (categories.Any(x => x is StatCategory.Fractured or StatCategory.Desecrated or StatCategory.Crafted))
        {
            PrimaryCategory = categories.FirstOrDefault(x => x is StatCategory.Fractured or StatCategory.Desecrated or StatCategory.Crafted);
            SecondaryCategory = categories.FirstOrDefault(x => x == StatCategory.Explicit);
            UsePrimaryCategory = PrimaryCategory is StatCategory.Fractured;
        }
        else
        {
            UsePrimaryCategory = false;
            PrimaryCategory = Stat.ApiInformation.FirstOrDefault()?.Category ?? StatCategory.Undefined;
            SecondaryCategory = StatCategory.Undefined;
        }
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

    public StatCategory PrimaryCategory { get; init; }

    public StatCategory SecondaryCategory { get; init; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public double NormalizeValue => Stat.AverageValue;

    public bool NormalizeEnabled => true;

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked || Stat.ApiInformation.Count == 0)
        {
            return;
        }

        if (Stat.ApiInformation.Count == 1)
        {
            query.GetOrCreateStatGroup(StatType.And).Filters.Add(new StatFilters()
            {
                Id = Stat.ApiInformation.First().Id,
                Value = new StatFilterValue(this),
            });
        }
        else
        {
            var stats = Stat.ApiInformation.ToList();
            if (UsePrimaryCategory)
            {
                stats = stats.Where(x => x.Category == PrimaryCategory).ToList();
            }
            else if (SecondaryCategory != StatCategory.Undefined)
            {
                stats = stats.Where(x => x.Category == SecondaryCategory).ToList();
            }

            var countGroup = query.GetOrCreateStatGroup(StatType.Count);
            foreach (var stat in stats)
            {
                countGroup.Filters.Add(new StatFilters()
                {
                    Id = stat.Id,
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
