using System.Text.Json.Serialization;

namespace Sidekick.Apis.GitHub.Models
{
    public class Asset
    {
        [JsonConstructor]
        public Asset(string? url, string? name, string? downloadUrl)
        {
            Url = url;
            Name = name;
            DownloadUrl = downloadUrl;
        }

        public string? Url { get; init; }

        public string? Name { get; init; }

        [JsonPropertyName("browser_download_url")]
        public string? DownloadUrl { get; init; }
    }
}
