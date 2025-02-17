using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Results;

public class ResultSocket
{
    public int Group { get; set; }

    [JsonPropertyName("sColour")]
    public string? ColourString { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("item")]
    public string? Item { get; set; }
}
