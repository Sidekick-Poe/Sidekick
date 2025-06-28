using System.Text.Json.Serialization;

namespace Sidekick.Apis.PoeNinja.Models;

/// <summary>
/// https://poe.ninja/api/data/index-state
/// </summary>
public class NinjaIndexState
{
    [JsonPropertyName("economyLeagues")]
    public List<NinjaEconomyLeague> EconomyLeagues { get; set; } = [];
}
