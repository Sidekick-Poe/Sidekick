using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
namespace Sidekick.Apis.Poe.Trade.ApiStats;

public class StatDefinition
(
    StatCategory category,
    string id,
    string text,
    string fuzzyText,
    Regex pattern
)
{
    public StatCategory Category { get; } = category;

    public string Id { get; } = id;

    public string Text { get; } = text;

    public string FuzzyText { get; } = fuzzyText;

    public Regex Pattern { get; } = pattern;

    public string? OptionText { get; init; }

    public int? OptionId { get; init; }

    public bool IsOption => OptionId != null || !string.IsNullOrEmpty(OptionText);

    private int? lineCount;

    public int LineCount => lineCount ??= OptionText != null ? OptionText.Split('\n').Length : Text.Split('\n').Length;

    public List<StatDefinition> SecondaryDefinitions { get; set; } = [];
}
