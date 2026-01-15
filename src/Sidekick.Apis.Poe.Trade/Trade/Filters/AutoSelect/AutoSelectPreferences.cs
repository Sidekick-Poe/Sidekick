using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;

#pragma warning disable CS0659

public class AutoSelectPreferences : IEquatable<AutoSelectPreferences>
{
    [JsonPropertyName("mode")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AutoSelectMode Mode { get; set; }

    [JsonPropertyName("rules")]
    public List<AutoSelectRule> Rules { get; set; } = [];

    public bool Equals(AutoSelectPreferences? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Mode == other.Mode && Rules.SequenceEqual(other.Rules);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((AutoSelectPreferences)obj);
    }

    public bool? ShouldCheck(Item item, TradeFilter filter)
    {
        if (Mode == AutoSelectMode.Always) return true;
        if (Mode == AutoSelectMode.Never) return false;
        if (Mode == AutoSelectMode.Any) return null;

        var matchingRule = Rules.FirstOrDefault(rule => rule.Conditions.All(c => ConditionMatches(c, item, filter)));
        return matchingRule?.Checked;
    }

    private static bool ConditionMatches(AutoSelectCondition condition, Item item, TradeFilter filter)
    {
        object? value = condition.Type switch
        {
            AutoSelectConditionType.AreaLevel => item.Properties.AreaLevel,
            AutoSelectConditionType.Armour => item.Properties.Armour,
            AutoSelectConditionType.AttacksPerSecond => item.Properties.AttacksPerSecond,
            AutoSelectConditionType.ItemClass => item.Properties.ItemClass,
            AutoSelectConditionType.ItemLevel => item.Properties.ItemLevel,
            AutoSelectConditionType.Quality => item.Properties.Quality,
            AutoSelectConditionType.Rarity => item.Properties.Rarity,
            AutoSelectConditionType.Corrupted => item.Properties.Corrupted,
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
            AutoSelectConditionType.StatCategory when filter is StatFilter statFilter => statFilter.PrimaryCategory,
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
            AutoSelectComparisonType.GreaterThan => Compare(value, condition.Value) > 0,
            AutoSelectComparisonType.LesserThan => Compare(value, condition.Value) < 0,
            AutoSelectComparisonType.GreaterThanOrEqual => Compare(value, condition.Value) >= 0,
            AutoSelectComparisonType.LesserThanOrEqual => Compare(value, condition.Value) <= 0,
            AutoSelectComparisonType.Equals => Compare(value, condition.Value) == 0,
            AutoSelectComparisonType.DoesNotEqual => Compare(value, condition.Value) != 0,
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

        return Regex.IsMatch(value, conditionValue, RegexOptions.CultureInvariant | RegexOptions.Multiline);
    }

    private static int Compare(object expressionValue, string? conditionValue)
    {
        var value = expressionValue.ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(conditionValue)) return 0;
        if (string.IsNullOrEmpty(value)) return -1;
        if (string.IsNullOrEmpty(conditionValue)) return 1;

        if (expressionValue is Enum enumValue)
        {
            return string.CompareOrdinal(enumValue.ToString(), conditionValue);
        }

        if (expressionValue is IComparable comparable)
        {
            var convertedValue = Convert.ChangeType(conditionValue, expressionValue.GetType());
            return comparable.CompareTo(convertedValue);
        }

        try
        {
            var expressionDouble = Convert.ToDouble(expressionValue);
            var conditionDouble = Convert.ToDouble(conditionValue);
            return expressionDouble.CompareTo(conditionDouble);
        }
        catch (Exception)
        {
            return 0;
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
