using System.Text.Json.Serialization;

namespace Sidekick.Wpf.Browser;

public class DevToolsParameters
{
    [JsonPropertyName("request")]
    public DevToolsRequest? Request { get; set; }
}
