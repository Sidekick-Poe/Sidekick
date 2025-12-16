using System.Text.Json.Serialization;
using Sidekick.Apis.Poe.Items;

namespace Sidekick.Apis.Poe.Trade.AutoSelect.Models;

public enum AutoSelectConditionType
{
    ItemRarityIs,
    ItemClassIs,
    FilterTextRegex,
}

public class AutoSelectCondition
{
    public AutoSelectConditionType Type { get; set; }

    // For ItemRarityIs and ItemClassIs we store string names
    public string? Value { get; set; }
}

public class AutoSelectRule
{
    // All conditions must pass for the rule to match
    public List<AutoSelectCondition> All { get; set; } = [];
}
