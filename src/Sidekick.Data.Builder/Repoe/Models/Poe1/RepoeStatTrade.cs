using System.Text.Json.Serialization;
namespace Sidekick.Data.Builder.Repoe.Models.Poe1;

public class RepoeStatTrade
{
    public required string Id { get; set; }

    public required string Text { get; set; }

    public required string Type { get; set; }

    [JsonPropertyName("option")]
    public RepoeStatTradeOptions? Options { get; set; }

    [JsonIgnore]
    public int? OptionValue
    {
        get
        {
            var split = Id.Split('|', 2);
            if (split.Length == 2 && int.TryParse(split[1], out var value))
            {
                return value;
            }

            return null;
        }
    }
}
