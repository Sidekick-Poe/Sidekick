using System.Text.Json.Serialization;
using Sidekick.Apis.PoeWiki.JsonConverters;

namespace Sidekick.Apis.PoeWiki.ApiModels
{
    public class MapItemResult
    {
        public string? Name { get; set; }

        /// <summary>
        /// "1" = true, "0" = false.
        /// </summary>
        [JsonPropertyName("drop enabled")]
        [JsonConverter(typeof(StringToBooleanJsonConverter))]
        public bool DropEnabled { get; set; }

        [JsonPropertyName("flavour text")]
        public string? FlavourText { get; set; }

        [JsonPropertyName("drop text")]
        public string? DropText { get; set; }

        [JsonPropertyName("drop monsters")]
        [JsonConverter(typeof(StringListToListOfStringsJsonConverter))]
        public List<string> DropMonsters { get; set; } = new();

        [JsonPropertyName("drop areas")]
        [JsonConverter(typeof(StringListToListOfStringsJsonConverter))]
        public List<string> DropAreas { get; set; } = new();
    }
}
