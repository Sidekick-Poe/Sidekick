using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Sidekick.Game.Stats;

public class StatDefinition
{
    public required string Text { get; set; }

    public bool Negate { get; set; }

    public bool MatchedFuzzily { get; set; }

    public double? Value { get; set; }

    public int Lines { get; set; }

    public List<string>? TradeIds { get; set; }

    [JsonIgnore]
    public Regex? Pattern { get; set; }

    [JsonPropertyName("pattern")]
    public string? PatternValue
    {
        get
        {
            return Pattern?.ToString();
        }
        init
        {
            Pattern = value == null ? null : new Regex(value);
        }
    }

    public override string ToString() => Text;
}
