using System.Text.Json.Serialization;

namespace Sidekick.Wpf.Cloudflare;

public class DevToolsParameters
{
    [JsonPropertyName("request")]
    public DevToolsRequest? Request { get; set; }
}
