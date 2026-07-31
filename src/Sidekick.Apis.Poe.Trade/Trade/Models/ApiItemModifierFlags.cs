using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe.Trade.Trade.Models;

public class ApiItemModifierFlags
{
    [JsonPropertyName("crafted")]
    public bool Crafted { get; set; }

    [JsonPropertyName("desecrated")]
    public bool Desecrated { get; set; }

    [JsonPropertyName("fractured")]
    public bool Fractured { get; set; }

    [JsonPropertyName("mutated")]
    public bool Mutated { get; set; }
}
