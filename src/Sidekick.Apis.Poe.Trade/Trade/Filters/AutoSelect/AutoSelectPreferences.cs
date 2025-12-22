using System.Text.Json.Serialization;
using Sidekick.Apis.Poe.Items;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;

public class AutoSelectPreferences
{
    [JsonPropertyName("rules")]
    public AutoSelectMode Mode { get; set; }

    [JsonPropertyName("rules")]
    public List<AutoSelectRule> Rules { get; set; }

    public bool ShouldCheck(Item item)
    {
        if (Mode == AutoSelectMode.Always) return true;
        if (Mode == AutoSelectMode.Never) return false;

        if (Rules.Count == 0) return false;

        foreach (var rule in Rules)
        {
            if (RuleMatches(rule, item)) return rule.Checked;
        }

        return false;

    }

    private static bool RuleMatches(AutoSelectRule rule, Item item)
    {
        foreach (var condition in rule.Conditions)
        {
            if (!ConditionMatches(condition, item)) return false;
        }

        return true;
    }

    private static bool ConditionMatches(AutoSelectCondition condition, Item item)
    {
        if (condition.Expression == null)
        {
            return true;
        }

        var expressionValue = condition.Expression.Compile().Invoke(item);
        if (expressionValue == null && condition.Value == null)
        {
            return true;
        }

        if (expressionValue == null || condition.Value == null)
        {
            return false;
        }

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
    Conditionally,
}
