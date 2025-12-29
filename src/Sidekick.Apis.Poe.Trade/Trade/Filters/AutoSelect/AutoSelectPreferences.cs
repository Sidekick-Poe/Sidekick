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
            AutoSelectConditionType.Stat => filter.Text,
            AutoSelectConditionType.StatCategory when filter is StatFilter statFilter => statFilter.PrimaryCategory,
            AutoSelectConditionType.SocketCount => item.Properties.GetMaximumNumberOfLinks(),
            _ => null,
        };
        if (value == null && condition.Value == null) return true;
        if (value == null || condition.Value == null) return false;

        return condition.ComparisonType switch
        {
            AutoSelectComparisonType.GreaterThan => Compare(value, condition.Value) > 0,
            AutoSelectComparisonType.LesserThan => Compare(value, condition.Value) < 0,
            AutoSelectComparisonType.GreaterThanOrEqual => Compare(value, condition.Value) >= 0,
            AutoSelectComparisonType.LesserThanOrEqual => Compare(value, condition.Value) <= 0,
            AutoSelectComparisonType.Equals => Equals(value, condition.Value),
            AutoSelectComparisonType.IsContainedIn => IsContainedIn(value, condition.Value),
            AutoSelectComparisonType.MatchesRegex => IsMatch(value, condition),
            AutoSelectComparisonType.DoesNotMatchRegex => !IsMatch(value, condition),
            _ => false,
        };
    }

    private static bool IsMatch(object expressionValue, object conditionValue)
    {
        var value = expressionValue.ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(value)) return false;

        if (conditionValue is not string stringValue) return false;

        return Regex.IsMatch(value, stringValue);
    }

    private static int Compare(object expressionValue, object conditionValue)
    {
        if (expressionValue is IComparable comparable)
        {
            // Ensure types match for comparison
            var convertedValue = Convert.ChangeType(conditionValue, expressionValue.GetType());
            return comparable.CompareTo(convertedValue);
        }

        var expressionDouble = Convert.ToDouble(expressionValue);
        var conditionDouble = Convert.ToDouble(conditionValue);
        return expressionDouble.CompareTo(conditionDouble);
    }

    private static bool IsContainedIn(object expressionValue, object conditionValue)
    {
        if (conditionValue is not System.Collections.IEnumerable enumerable) return false;

        return enumerable.Cast<object?>().Contains(expressionValue);
    }
}
