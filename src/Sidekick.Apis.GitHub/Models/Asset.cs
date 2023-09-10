using System.Text.Json.Serialization;

namespace Sidekick.Apis.GitHub.Models
{
    public record Asset
    {
        public string? Url { get; init; }

        public string? Name { get; init; }

        [JsonPropertyName("browser_download_url")]
        public string? DownloadUrl { get; init; }
    }
}
