using System.Text.Json.Serialization;

namespace Sidekick.Apis.GitHub.Models
{
    public class GitHubRelease
    {
        [JsonConstructor]
        public GitHubRelease(string? tag, string? name, bool prerelease, Asset[]? assets)
        {
            Tag = tag;
            Name = name;
            Prerelease = prerelease;
            Assets = assets;
        }

        [JsonPropertyName("tag_name")]
        public string? Tag { get; init; }

        public string? Name { get; init; }

        public bool Prerelease { get; init; }

        public Asset[]? Assets { get; init; }
    }
}
