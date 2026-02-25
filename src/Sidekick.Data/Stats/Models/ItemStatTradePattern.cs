using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
namespace Sidekick.Data.Stats.Models;

public class ItemStatTradePattern
{
    public required string Id { get; set; }

    public required string Text { get; set; }

    public required string Type { get; set; }

    public ItemStatOption? Option { get; set; }

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
