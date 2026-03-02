using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Settings;
using Sidekick.Data.Items;

namespace Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;

public class AutoSelectPreferences
{
    public const string DefaultNormalizeBySettingKey = "Trade_Filter_Default_NormalizeBy";
    public const string DefaultFillMinSettingKey = "Trade_Filter_Default_FillMin";
    public const string DefaultFillMaxSettingKey = "Trade_Filter_Default_FillMax";
    public const string DefaultSelectCategoriesSettingKey = "Trade_Filter_Default_SelectCategories";

    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters =
        {
            new JsonStringEnumConverter(),
        },
    };

    public static AutoSelectPreferences Create(bool? isChecked, double? normalizeBy = null) => new()
    {
        Rules =
        [
            new AutoSelectRule
            {
                Checked = isChecked,
                NormalizeBy = normalizeBy,
                FillMinRange = normalizeBy.HasValue ? true : null,
            },
        ],
    };

    [JsonPropertyName("mode")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AutoSelectMode Mode { get; set; }

    [JsonPropertyName("rules")]
    public List<AutoSelectRule> Rules { get; set; } = [];

    public async Task<AutoSelectResult> GetResult(Item item, TradeFilter filter, ISettingsService settingsService)
    {
        var normalizeBy = await settingsService.GetDouble(DefaultNormalizeBySettingKey);
        var fillMin = await settingsService.GetBool(DefaultFillMinSettingKey);
        var fillMax = await settingsService.GetBool(DefaultFillMaxSettingKey);

        var matchingRule = Rules.FirstOrDefault(rule => rule.Conditions.Count == 0 || rule.Conditions.All(c => ConditionMatches(c, item, filter)));
        AutoSelectResult result;
        if (matchingRule == null)
        {
            result = new AutoSelectResult()
            {
                Checked = false,
                NormalizeBy = normalizeBy,
                FillMaxRange = fillMax,
                FillMinRange = fillMin,
            };
        }
        else
        {
            result = new AutoSelectResult()
            {
                Checked = matchingRule.Checked,
                NormalizeBy = matchingRule.NormalizeBy ?? normalizeBy,
                FillMaxRange = matchingRule.FillMaxRange ?? fillMax,
                FillMinRange = matchingRule.FillMinRange ?? fillMin,
            };
        }

        if (filter is StatFilter statFilter)
        {
            var selectCategories = await settingsService.GetObject<List<StatCategory>>(DefaultSelectCategoriesSettingKey) ?? [];
            if (matchingRule?.SelectCategories != null) selectCategories = matchingRule.SelectCategories;
            result.SelectCategory = statFilter.Stat.Category.HasExplicitStat() && selectCategories.Contains(statFilter.Stat.Category);
        }

        return result;
    }

    private static bool ConditionMatches(AutoSelectCondition condition, Item item, TradeFilter filter)
    {
        object? value = condition.Type switch
        {
            AutoSelectConditionType.AreaLevel => item.Properties.AreaLevel,
            AutoSelectConditionType.Armour => item.Properties.Armour,
            AutoSelectConditionType.AttacksPerSecond => item.Properties.AttacksPerSecond,
            AutoSelectConditionType.Blighted => item.Properties.Blighted,
            AutoSelectConditionType.BlightRavaged => item.Properties.BlightRavaged,
            AutoSelectConditionType.ItemClass => item.Properties.ItemClass,
            AutoSelectConditionType.ItemLevel => item.Properties.ItemLevel,
            AutoSelectConditionType.Quality => item.Properties.Quality,
            AutoSelectConditionType.Rarity => item.Properties.Rarity,
            AutoSelectConditionType.Corrupted => item.Properties.Corrupted,
            AutoSelectConditionType.Unidentified => item.Properties.Unidentified,
            AutoSelectConditionType.Spirit => item.Properties.Spirit,
            AutoSelectConditionType.Foulborn => item.Properties.Foulborn,
            AutoSelectConditionType.EvasionRating => item.Properties.EvasionRating,
            AutoSelectConditionType.EnergyShield => item.Properties.EnergyShield,
            AutoSelectConditionType.BlockChance => item.Properties.BlockChance,
            AutoSelectConditionType.MapTier => item.Properties.MapTier,
            AutoSelectConditionType.ItemQuantity => item.Properties.ItemQuantity,
            AutoSelectConditionType.ItemRarity => item.Properties.ItemRarity,
            AutoSelectConditionType.MagicMonsters => item.Properties.MagicMonsters,
            AutoSelectConditionType.MonsterPackSize => item.Properties.MonsterPackSize,
            AutoSelectConditionType.RareMonsters => item.Properties.RareMonsters,
            AutoSelectConditionType.CriticalHitChance => item.Properties.CriticalHitChance,
            AutoSelectConditionType.PhysicalDps => item.Properties.PhysicalDpsWithQuality,
            AutoSelectConditionType.TotalDps => item.Properties.TotalDpsWithQuality,
            AutoSelectConditionType.GemLevel => item.Properties.GemLevel,
            AutoSelectConditionType.AnyStat => string.Join('\n', item.Stats.Select(x => x.Text)),
            AutoSelectConditionType.Text => filter.Text,
            AutoSelectConditionType.StatCategory when filter is StatFilter statFilter => statFilter.Stat.Category,
            AutoSelectConditionType.SocketCount => item.Properties.GetMaximumNumberOfLinks(),
            AutoSelectConditionType.Value when filter is DoublePropertyFilter doubleFilter => doubleFilter.Value,
            AutoSelectConditionType.Value when filter is IntPropertyFilter intFilter => intFilter.Value,
            AutoSelectConditionType.Value when filter is OptionFilter optionFilter => optionFilter.Value,
            AutoSelectConditionType.Value when filter is PseudoFilter pseudoFilter => pseudoFilter.Stat.Value,
            AutoSelectConditionType.Value when filter is StatFilter statFilter => statFilter.Stat.AverageValue,
            AutoSelectConditionType.Value when filter is StringPropertyFilter stringFilter => stringFilter.Value,
            _ => null,
        };
        if (value == null) return false;

        return condition.Comparison switch
        {
            AutoSelectComparisonType.True => value is true,
            AutoSelectComparisonType.False => value is false,
            AutoSelectComparisonType.GreaterThan => TryCompare(value, condition.Value, out var result) && result > 0,
            AutoSelectComparisonType.LesserThan => TryCompare(value, condition.Value, out var result) && result < 0,
            AutoSelectComparisonType.GreaterThanOrEqual => TryCompare(value, condition.Value, out var result) && result >= 0,
            AutoSelectComparisonType.LesserThanOrEqual => TryCompare(value, condition.Value, out var result) && result <= 0,
            AutoSelectComparisonType.Equals => TryCompare(value, condition.Value, out var result) && result == 0,
            AutoSelectComparisonType.DoesNotEqual => TryCompare(value, condition.Value, out var result) && result != 0,
            AutoSelectComparisonType.IsContainedIn => IsContainedIn(value, condition.Value),
            AutoSelectComparisonType.IsNotContainedIn => !IsContainedIn(value, condition.Value),
            AutoSelectComparisonType.MatchesRegex => IsMatch(value, condition.Value),
            AutoSelectComparisonType.DoesNotMatchRegex => !IsMatch(value, condition.Value),
            _ => false,
        };
    }

    private static bool IsMatch(object expressionValue, string? conditionValue)
    {
        var value = expressionValue.ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(value) || conditionValue == null) return false;

        return Regex.IsMatch(value, conditionValue, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline);
    }

    private static bool TryCompare(object expressionValue, string? conditionValue, out int result)
    {
        try
        {
            var value = expressionValue.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(conditionValue))
            {
                result = 0;
                return true;
            }

            if (string.IsNullOrEmpty(value))
            {
                result = -1;
                return true;
            }

            if (string.IsNullOrEmpty(conditionValue))
            {
                result = 1;
                return true;
            }

            if (expressionValue is Enum enumValue)
            {
                result = string.CompareOrdinal(enumValue.ToString(), conditionValue);
                return true;
            }

            if (expressionValue is IComparable comparable)
            {
                var convertedValue = Convert.ChangeType(conditionValue, expressionValue.GetType());
                result = comparable.CompareTo(convertedValue);
                return true;
            }

            var expressionDouble = Convert.ToDouble(expressionValue);
            var conditionDouble = Convert.ToDouble(conditionValue);
            result = expressionDouble.CompareTo(conditionDouble);
            return true;
        }
        catch (Exception)
        {
            result = 0;
            return false;
        }
    }

    private static bool IsContainedIn(object expressionValue, string? conditionValue)
    {
        try
        {
            var items = JsonSerializer.Deserialize<List<string>>(conditionValue ?? "[]");
            return items?.Contains(expressionValue.ToString() ?? string.Empty) ?? false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
