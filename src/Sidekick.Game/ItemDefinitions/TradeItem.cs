using System.Text.Json.Serialization;

namespace Sidekick.Game.ItemDefinitions;

public class TradeItem
{
    public string? Text { get; init; }
    public string? Name { get; init; }
    public string? Type { get; init; }

    [JsonPropertyName("cat")]
    public string? Category { get; init; }

    [JsonPropertyName("disc")]
    public string? Discriminator { get; init; }
}