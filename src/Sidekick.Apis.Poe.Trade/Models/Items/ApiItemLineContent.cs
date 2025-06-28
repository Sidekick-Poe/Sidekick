using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Models.Items;

public class ApiItemLineContent
{
    public string? Name { get; set; }

    public string? Icon { get; set; }

    [JsonPropertyName("values")]
    public List<List<JsonElement>> Values { get; set; } = new();

    public int DisplayMode { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }
}
