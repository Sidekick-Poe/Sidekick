using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters
{
    internal class ArmourFilterGroup
    {
        public bool Disabled { get; set; }

        [JsonPropertyName("filters")]
        public ArmourFilters Filters { get; set; } = new();
    }
}
