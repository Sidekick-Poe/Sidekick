using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Results
{
    public class Hashes
    {
        [JsonPropertyName("implicit")]
        public List<List<JsonElement>> Implicit { get; set; } = new();

        [JsonPropertyName("explicit")]
        public List<List<JsonElement>> Explicit { get; set; } = new();

        [JsonPropertyName("crafted")]
        public List<List<JsonElement>> Crafted { get; set; } = new();

        [JsonPropertyName("enchant")]
        public List<List<JsonElement>> Enchant { get; set; } = new();

        [JsonPropertyName("pseudo")]
        public List<List<JsonElement>> Pseudo { get; set; } = new();

        [JsonPropertyName("fractured")]
        public List<List<JsonElement>> Fractured { get; set; } = new();

        [JsonPropertyName("scourge")]
        public List<List<JsonElement>> Scourge { get; set; } = new();

        [JsonPropertyName("monster")]
        public List<List<JsonElement>> Monster { get; set; } = new();
        [JsonPropertyName("necropolis")]
        public List<List<JsonElement>> Necropolis { get; set; } = new();
    }
}
