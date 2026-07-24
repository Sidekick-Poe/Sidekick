using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe.Trade.Trade.Models;

public class ApiItemModifier
{
    public string? Description { get; set; }
    public string? Hash { get; set; }

    [JsonPropertyName("mods")]
    public List<ApiItemModifierDetail> Details { get; set; } = [];
}
