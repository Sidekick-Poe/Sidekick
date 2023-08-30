using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Filters
{
    public class RequirementFilter
    {
        [JsonPropertyName("lvl")]
        public SearchFilterValue? Level { get; set; }

        [JsonPropertyName("dex")]
        public SearchFilterValue? Dexterity { get; set; }

        [JsonPropertyName("str")]
        public SearchFilterValue? Strength { get; set; }

        [JsonPropertyName("int")]
        public SearchFilterValue? Intelligence { get; set; }
    }
}
