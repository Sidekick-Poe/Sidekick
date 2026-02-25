using System.Text.Json.Serialization;
namespace Sidekick.Data.Trade.Models.Raw;

public class RawTradeItem
{
    public string? Name { get; init; }

    public string? Type { get; init; }

    public string? Text { get; init; }

    [JsonPropertyName("disc")]
    public string? Discriminator { get; init; }

    public RawTradeItemFlags? Flags { get; init; }

    [JsonIgnore]
    public string? Category { get; set; }

    [JsonIgnore]
    public bool IsUnique => Flags?.Unique ?? false;
}
