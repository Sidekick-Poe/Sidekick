using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;

public class MapFilters
{
    [JsonPropertyName("map_revives")]
    public StatFilterValue? RevivesAvailable { get; set; }

    [JsonPropertyName("map_iiq")]
    public StatFilterValue? ItemQuantity { get; set; }

    [JsonPropertyName("map_iir")]
    public StatFilterValue? ItemRarity { get; set; }

    [JsonPropertyName("area_level")]
    public StatFilterValue? AreaLevel { get; set; }

    [JsonPropertyName("map_tier")]
    public StatFilterValue? MapTier { get; set; }

    [JsonPropertyName("map_completion_reward")]
    public SearchFilterOption? Reward { get; set; }

    [JsonPropertyName("map_packsize")]
    public StatFilterValue? MonsterPackSize { get; set; }

    [JsonPropertyName("map_magic_monsters")]
    public StatFilterValue? MagicMonsters { get; set; }

    [JsonPropertyName("map_rare_monsters")]
    public StatFilterValue? RareMonsters { get; set; }

    [JsonPropertyName("map_bonus")]
    public StatFilterValue? WaystoneDropChance { get; set; }

    [JsonPropertyName("map_blighted")]
    public SearchFilterOption? Blighted { get; set; }

    [JsonPropertyName("map_uberblighted")]
    public SearchFilterOption? BlightRavavaged { get; set; }

    [JsonPropertyName("map_elder")]
    public SearchFilterOption? Elder { get; set; }

    [JsonPropertyName("map_shaped")]
    public SearchFilterOption? Shaped { get; set; }
}
