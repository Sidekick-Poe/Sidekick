using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters
{
    internal class EquipmentFilterGroup
    {
        public bool Disabled { get; set; }

        [JsonPropertyName("filters")]
        public EquipmentFilters Filters { get; set; } = new();
    }
}
