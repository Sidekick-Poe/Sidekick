using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe2Scout.Items.Models;

public class ScoutItemMetadata
{
    public string? Name { get; set; }

    [JsonPropertyName("base_type")]
    public string? Type { get; set; }
}
