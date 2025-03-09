using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters;

public class SearchFilters
{
    [JsonPropertyName("type_filters")]
    public TypeFilterGroup? Type_filters { get; set; }

    [JsonIgnore]
    public TypeFilterGroup TypeFilters => Type_filters ??= new();

    [JsonPropertyName("trade_filters")]
    public TradeFilterGroup? Trade_filters { get; set; }

    [JsonIgnore]
    public TradeFilterGroup TradeFilters => Trade_filters ??= new();

    [JsonPropertyName("misc_filters")]
    public MiscFilterGroup? Misc_filters { get; set; }

    [JsonIgnore]
    public MiscFilterGroup MiscFilters => Misc_filters ??= new();

    [JsonPropertyName("weapon_filters")]
    public WeaponFilterGroup? Weapon_filters { get; set; }

    [JsonIgnore]
    public WeaponFilterGroup WeaponFilters => Weapon_filters ??= new();

    [JsonPropertyName("armour_filters")]
    public ArmourFilterGroup? Armour_filters { get; set; }

    [JsonIgnore]
    public ArmourFilterGroup ArmourFilters => Armour_filters ??= new();

    [JsonPropertyName("equipment_filters")]
    public EquipmentFilterGroup? Equipment_filters { get; set; }

    [JsonIgnore]
    public EquipmentFilterGroup EquipmentFilters => Equipment_filters ??= new();

    [JsonPropertyName("socket_filters")]
    public SocketFilterGroup? SocketFilters { get; set; }

    [JsonPropertyName("req_filters")]
    public RequirementFilterGroup? RequirementFilters { get; set; }

    [JsonPropertyName("map_filters")]
    public MapFilterGroup? Map_filters { get; set; }

    [JsonIgnore]
    public MapFilterGroup MapFilters => Map_filters ??= new();

}
