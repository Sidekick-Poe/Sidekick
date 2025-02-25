using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Results;

public class Hashes
{
    [JsonPropertyName("implicit")]
    public List<List<JsonElement>> Implicit { get; set; } = [];

    [JsonPropertyName("explicit")]
    public List<List<JsonElement>> Explicit { get; set; } = [];

    [JsonPropertyName("crafted")]
    public List<List<JsonElement>> Crafted { get; set; } = [];

    [JsonPropertyName("enchant")]
    public List<List<JsonElement>> Enchant { get; set; } = [];

    [JsonPropertyName("rune")]
    public List<List<JsonElement>> Rune { get; set; } = [];

    [JsonPropertyName("pseudo")]
    public List<List<JsonElement>> Pseudo { get; set; } = [];

    [JsonPropertyName("fractured")]
    public List<List<JsonElement>> Fractured { get; set; } = [];

    [JsonPropertyName("scourge")]
    public List<List<JsonElement>> Scourge { get; set; } = [];

    [JsonPropertyName("monster")]
    public List<List<JsonElement>> Monster { get; set; } = [];

    [JsonPropertyName("sanctum")]
    public List<List<JsonElement>> Sanctum { get; set; } = [];
}
