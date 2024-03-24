using System.Text.Json.Serialization;

namespace Sidekick.Apis.GitHub.Api;

internal record Asset
{
    public string? Name { get; init; }

    [JsonPropertyName("browser_download_url")]
    public string? DownloadUrl { get; init; }
}
