using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
namespace Sidekick.Data.Stats;

public static class StatDefinitionExtensions
{
    public static string GetStatId(this string id) => id.Split('#').First();

    public static int? GetStatOption(this string id)
    {
        var option = id.Split('#').ElementAtOrDefault(1);
        if (int.TryParse(option, out var result)) return result;
        return null;
    }
}

public class StatDefinition
{
    public required string Text { get; set; }

    public bool Negate { get; set; }

    public bool MatchedFuzzily { get; set; }

    public double? Value { get; set; }

    public int Lines { get; set; }

    public List<string>? TradeIds { get; set; }

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

    public override string ToString() => Text;
}
