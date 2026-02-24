using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
namespace Sidekick.Data.Trade.Models;

public class StatPattern
{
    [JsonIgnore]
    public required Regex Regex { get; set; }

    public bool Negated { get; set; }

    [JsonPropertyName("pattern")]
    public string Text
    {
        get
        {
            return Regex.ToString();
        }
        set
        {
            Regex = new Regex(value);
        }
    }
}
