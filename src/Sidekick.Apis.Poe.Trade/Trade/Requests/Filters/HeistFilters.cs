using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;

public class HeistFilters
{
    [JsonPropertyName("heist_wings")]
    public StatFilterValue? WingsRevealed { get; set; }

    [JsonPropertyName("heist_max_wings")]
    public StatFilterValue? WingsTotal { get; set; }

    [JsonPropertyName("heist_escape_routes")]
    public StatFilterValue? RoutesRevealed { get; set; }

    [JsonPropertyName("heist_max_escape_routes")]
    public StatFilterValue? RoutesTotal { get; set; }

    [JsonPropertyName("heist_reward_rooms")]
    public StatFilterValue? RoomsRevealed { get; set; }

    [JsonPropertyName("heist_max_reward_rooms")]
    public StatFilterValue? RoomsTotal { get; set; }

    [JsonPropertyName("heist_objective_value")]
    public SearchFilterOption? ObjectiveValue { get; set; }

    [JsonPropertyName("heist_lockpicking")]
    public StatFilterValue? Lockpicking { get; set; }

    [JsonPropertyName("heist_demolition")]
    public StatFilterValue? Demolition { get; set; }

    [JsonPropertyName("heist_agility")]
    public StatFilterValue? Agility { get; set; }

    [JsonPropertyName("heist_counter_thaumaturgy")]
    public StatFilterValue? CounterThaumaturgy { get; set; }

    [JsonPropertyName("heist_trap_disarmament")]
    public StatFilterValue? TrapDisarmament { get; set; }

    [JsonPropertyName("heist_perception")]
    public StatFilterValue? Perception { get; set; }

    [JsonPropertyName("heist_brute_force")]
    public StatFilterValue? BruteForce { get; set; }

    [JsonPropertyName("heist_deception")]
    public StatFilterValue? Deception { get; set; }

    [JsonPropertyName("heist_engineering")]
    public StatFilterValue? Engineering { get; set; }
}
