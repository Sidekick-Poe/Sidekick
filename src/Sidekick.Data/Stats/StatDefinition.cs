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

    public static bool HasStatOption(this string id) => id.Contains('#');
}

public class StatDefinition
{
    public List<string>? GameIds { get; init; }

    public required DataSource Source { get; set; }

    public required string Text { get; set; }

    public string? FuzzyText { get; set; }

    public bool Negate { get; set; }

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
