using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
namespace Sidekick.Data.Items;

public class ItemStatGamePattern
{
    public bool Negate { get; set; }

    public string? Text { get; set; }

    public int? Option { get; set; }

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
