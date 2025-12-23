using System.Text.Json.Serialization;
using Sidekick.Apis.Poe.Items;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;

#pragma warning disable CS0659

public class AutoSelectPreferences : IEquatable<AutoSelectPreferences>
{
    [JsonPropertyName("rules")]
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

    public bool? ShouldCheck(Item item)
    {
        if (Mode == AutoSelectMode.Always) return true;
        if (Mode == AutoSelectMode.Never) return false;
        if (Mode == AutoSelectMode.Any) return null;

        var matchingRule = Rules.FirstOrDefault(rule => rule.Conditions.All(c => ConditionMatches(c, item)));
        return matchingRule?.Checked;
    }

    private static bool ConditionMatches(AutoSelectCondition condition, Item item)
    {
        var expressionValue = condition.Expression?.Compile().Invoke(item);
        if (expressionValue == null && condition.Value == null) return true;
        if (expressionValue == null || condition.Value == null) return false;

        return condition.Type switch
        {
            AutoSelectConditionType.GreaterThan => Compare(expressionValue, condition.Value) > 0,
            AutoSelectConditionType.LesserThan => Compare(expressionValue, condition.Value) < 0,
            AutoSelectConditionType.GreaterThanOrEqual => Compare(expressionValue, condition.Value) >= 0,
            AutoSelectConditionType.LesserThanOrEqual => Compare(expressionValue, condition.Value) <= 0,
            AutoSelectConditionType.Equals => Equals(expressionValue, condition.Value),
            AutoSelectConditionType.IsContainedIn => IsContainedIn(expressionValue, condition.Value),
            _ => false,
        };
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
        if (conditionValue is System.Collections.IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                if (Equals(expressionValue, item))
                {
                    return true;
                }
            }
        }

        return false;
    }
}

public enum AutoSelectMode
{
    Always,
    Never,
    Any,
    Conditionally,
}
