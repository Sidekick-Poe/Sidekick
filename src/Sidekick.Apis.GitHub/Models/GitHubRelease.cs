using System.Text.Json.Serialization;

namespace Sidekick.Apis.GitHub.Models
{
    public class GitHubRelease
    {
        [JsonPropertyName("tag_name")]
        public string? Tag { get; set; }

        public string? Name { get; set; }

        public bool Prerelease { get; set; }

        public Asset[]? Assets { get; set; }
    }
}
