using System.Text.Json.Serialization;
namespace Sidekick.Data.Builder.Repoe.Models.Items;

public class RepoeUniqueItemVisual
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("dds_file")]
    public string? File { get; init; }
}
