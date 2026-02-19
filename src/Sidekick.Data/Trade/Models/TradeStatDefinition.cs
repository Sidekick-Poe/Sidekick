using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
namespace Sidekick.Data.Trade.Models;

public class TradeStatDefinition
{
    public required StatCategory Category { get; init; }

    public required string Id { get; init; }

    public required string Text { get; init; }

    public required string FuzzyText { get; init; }

    [JsonIgnore]
    public required Regex Pattern { get; set; }

    [JsonPropertyName("pattern")]
    public string PatternText
    {
        get
        {
            return Pattern.ToString();
        }
        set
        {
            Pattern = new Regex(value);
        }
    }

    public string? OptionText { get; init; }

    public int? OptionId { get; init; }

    [JsonIgnore]
    public bool IsOption => OptionId != null || !string.IsNullOrEmpty(OptionText);

    private int? lineCount;

    [JsonIgnore]
    public int LineCount => lineCount ??= OptionText != null ? OptionText.Split('\n').Length : Text.Split('\n').Length;

    public List<TradeStatDefinition>? SecondaryDefinitions { get; set; }
}
