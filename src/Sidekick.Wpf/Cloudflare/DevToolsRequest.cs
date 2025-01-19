using System.Text.Json.Serialization;

namespace Sidekick.Wpf.Cloudflare;

public class DevToolsRequest
{
    [JsonPropertyName("headers")]
    public Dictionary<string, string> Headers { get; set; } = [];
}
