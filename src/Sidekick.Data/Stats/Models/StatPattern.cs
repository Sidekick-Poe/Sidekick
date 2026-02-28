using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Sidekick.Data.Items.Models;
namespace Sidekick.Data.Stats.Models;

public class StatPattern
{
    public required StatSource Source { get; set; }

    public required string Text { get; set; }

    public string? FuzzyText { get; set; }

    public List<string> GameIds { get; set; } = [];

    public List<string> TradeIds { get; set; } = [];

    public required StatCategory Category { get; set; }

    public StatOption? Option { get; set; }

    public bool Negate { get; set; }

    public double? Value { get; set; }

    [JsonIgnore]
    public required Regex Pattern { get; set; }

    [JsonPropertyName("pattern")]
    public string PatternValue
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

    public int LineCount { get; set; }

    public override string ToString() => Text;
}