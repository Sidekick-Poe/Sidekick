using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Sidekick.Data.Items.Models;
namespace Sidekick.Data.Stats.Models;

public class TradeStatPattern
{
    public required string Id { get; set; }

    public required string Text { get; set; }

    public required StatCategory Category { get; set; }

    public StatOption? Option { get; set; }

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
}
