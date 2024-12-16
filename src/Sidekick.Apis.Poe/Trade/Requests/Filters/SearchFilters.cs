using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters
{
    internal class SearchFilters
    {
        [JsonPropertyName("type_filters")]
        public TypeFilterGroup TypeFilters { get; set; } = new();

        [JsonPropertyName("trade_filters")]
        public TradeFilterGroup? TradeFilters { get; set; }

        [JsonPropertyName("misc_filters")]
        public MiscFilterGroup? MiscFilters { get; set; }

        [JsonPropertyName("weapon_filters")]
        public WeaponFilterGroup? WeaponFilters { get; set; }

        [JsonPropertyName("armour_filters")]
        public ArmourFilterGroup? ArmourFilters { get; set; }

        [JsonPropertyName("equipment_filters")]
        public EquipmentFilterGroup? EquipmentFilters { get; set; }

        [JsonPropertyName("socket_filters")]
        public SocketFilterGroup? SocketFilters { get; set; }

        [JsonPropertyName("req_filters")]
        public RequirementFilterGroup? RequirementFilters { get; set; }

        [JsonPropertyName("map_filters")]
        public MapFilterGroup? MapFilters { get; set; }
    }
}
