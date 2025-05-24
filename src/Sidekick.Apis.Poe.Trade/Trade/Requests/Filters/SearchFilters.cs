using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;

public class SearchFilters
{
    [JsonPropertyName("type_filters")]
    public TypeFilterGroup? TypeFilters { get; set; }

    public TypeFilterGroup GetOrCreateTypeFilters() => TypeFilters ??= new();

    [JsonPropertyName("trade_filters")]
    public TradeFilterGroup? TradeFilters { get; set; }

    public TradeFilterGroup GetOrCreateTradeFilters() => TradeFilters ??= new();

    [JsonPropertyName("misc_filters")]
    public MiscFilterGroup? MiscFilters { get; set; }

    public MiscFilterGroup GetOrCreateMiscFilters() => MiscFilters ??= new();

    [JsonPropertyName("weapon_filters")]
    public WeaponFilterGroup? WeaponFilters { get; set; }

    public WeaponFilterGroup GetOrCreateWeaponFilters() => WeaponFilters ??= new();

    [JsonPropertyName("armour_filters")]
    public ArmourFilterGroup? ArmourFilters { get; set; }

    public ArmourFilterGroup GetOrCreateArmourFilters() => ArmourFilters ??= new();

    [JsonPropertyName("equipment_filters")]
    public EquipmentFilterGroup? EquipmentFilters { get; set; }

    public EquipmentFilterGroup GetOrCreateEquipmentFilters() => EquipmentFilters ??= new();

    [JsonPropertyName("socket_filters")]
    public SocketFilterGroup? SocketFilters { get; set; }

    public SocketFilterGroup GetOrCreateSocketFilters() => SocketFilters ??= new();

    [JsonPropertyName("req_filters")]
    public RequirementFilterGroup? RequirementFilters { get; set; }

    [JsonPropertyName("map_filters")]
    public MapFilterGroup? MapFilters { get; set; }

    public MapFilterGroup GetOrCreateMapFilters() => MapFilters ??= new();

}
