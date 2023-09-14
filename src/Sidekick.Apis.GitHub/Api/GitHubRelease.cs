using System.Text.Json.Serialization;

namespace Sidekick.Apis.GitHub.Api
{
    internal record GitHubRelease
    {
        [JsonPropertyName("tag_name")]
        public string? Tag { get; init; }

        public string? Name { get; init; }

        public bool Prerelease { get; init; }

        public Asset[]? Assets { get; init; }
    }
}
