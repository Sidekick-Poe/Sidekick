using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Sidekick.Data.Trade;
namespace Sidekick.Data.Stats;

public class StatDefinition
{
    public required StatSource Source { get; set; }

    public required string Text { get; set; }

    public string? FuzzyText { get; set; }

    public List<string> GameIds { get; set; } = [];

    public List<TradeStatDefinition> TradeStats { get; set; } = [];

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

    public int Lines { get; set; }

    public override string ToString() => Text;
}