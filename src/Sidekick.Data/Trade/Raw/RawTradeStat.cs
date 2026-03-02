using System.Text.Json.Serialization;
namespace Sidekick.Data.Trade.Raw;

public record RawTradeStat
{
    public required string Id { get; init; }
    public required string Text { get; set; }
    public required string Type { get; init; }

    [JsonPropertyName("option")]
    public RawTradeStatOptions? Options { get; set; }
}
