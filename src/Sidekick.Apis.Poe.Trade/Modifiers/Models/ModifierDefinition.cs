using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Models;

namespace Sidekick.Apis.Poe.Trade.Modifiers.Models;

public class ModifierDefinition
(
    ModifierCategory category,
    string apiId,
    string apiText,
    string fuzzyText,
    Regex pattern
)
{
    public ModifierCategory Category { get; } = category;

    public string ApiId { get; } = apiId;

    public string ApiText { get; } = apiText;

    public string FuzzyText { get; } = fuzzyText;

    public Regex Pattern { get; } = pattern;

    public string? OptionText { get; init; }

    public int? OptionId { get; init; }

    public bool IsOption => OptionId != null || !string.IsNullOrEmpty(OptionText);

    private int? lineCount;

    public int LineCount => lineCount ??= OptionText != null ? OptionText.Split('\n').Length : ApiText.Split('\n').Length;
}
