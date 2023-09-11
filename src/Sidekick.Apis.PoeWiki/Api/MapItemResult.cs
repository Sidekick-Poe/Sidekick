using System.Text.Json.Serialization;
using Sidekick.Apis.PoeWiki.JsonConverters;

namespace Sidekick.Apis.PoeWiki.Api
{
    public record MapItemResult
    {
        public string? Name { get; init; }

        /// <summary>
        /// "1" = true, "0" = false.
        /// </summary>
        [JsonPropertyName("drop enabled")]
        [JsonConverter(typeof(StringToBooleanJsonConverter))]
        public bool DropEnabled { get; init; }

        [JsonPropertyName("flavour text")]
        public string? FlavourText { get; init; }

        [JsonPropertyName("drop text")]
        public string? DropText { get; init; }

        [JsonPropertyName("drop monsters")]
        [JsonConverter(typeof(StringListToListOfStringsJsonConverter))]
        public List<string> DropMonsters { get; init; } = new();

        [JsonPropertyName("drop areas")]
        [JsonConverter(typeof(StringListToListOfStringsJsonConverter))]
        public List<string> DropAreas { get; init; } = new();
    }
}
