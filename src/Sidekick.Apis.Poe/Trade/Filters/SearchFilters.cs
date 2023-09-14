using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Filters
{
    internal class SearchFilters
    {
        [JsonPropertyName("misc_filters")]
        public MiscFilterGroup MiscFilters { get; set; } = new();

        [JsonPropertyName("weapon_filters")]
        public WeaponFilterGroup WeaponFilters { get; set; } = new();

        [JsonPropertyName("armour_filters")]
        public ArmorFilterGroup ArmourFilters { get; set; } = new();

        [JsonPropertyName("socket_filters")]
        public SocketFilterGroup SocketFilters { get; set; } = new();

        [JsonPropertyName("req_filters")]
        public RequirementFilterGroup RequirementFilters { get; set; } = new();

        [JsonPropertyName("type_filters")]
        public TypeFilterGroup TypeFilters { get; set; } = new();

        [JsonPropertyName("map_filters")]
        public MapFilterGroup MapFilters { get; set; } = new();

        [JsonPropertyName("trade_filters")]
        public TradeFilterGroup TradeFilters { get; set; } = new();
    }
}
