using System.Text.Json.Serialization;

namespace Sidekick.Apis.GitHub.Api
{
    public record Asset
    {
        public string? Url { get; init; }

        public string? Name { get; init; }

        [JsonPropertyName("browser_download_url")]
        public string? DownloadUrl { get; init; }
    }
}
