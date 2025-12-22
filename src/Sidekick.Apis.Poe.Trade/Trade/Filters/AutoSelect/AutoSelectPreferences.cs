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

    }
}

public enum AutoSelectMode
{
    Always,
    Never,
    Conditionally,
}
