using System.Text.Json.Serialization;

namespace Sidekick.Apis.GitHub.Models
{
    public class Asset
    {
        public string? Url { get; set; }

        public string? Name { get; set; }

        [JsonPropertyName("browser_download_url")]
        public string? DownloadUrl { get; set; }
    }
}
