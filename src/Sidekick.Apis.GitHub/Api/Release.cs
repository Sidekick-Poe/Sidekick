using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Sidekick.Apis.GitHub.Api;

internal record Release
{
    [JsonPropertyName("tag_name")]
    public string? Tag { get; init; }

    public string? Name { get; init; }

    public bool Prerelease { get; init; }

    [JsonIgnore]
    public Version? Version
    {
        get
        {
            if (string.IsNullOrEmpty(Tag))
            {
                return null;
            }

            return new Version(Regex.Match(Tag, @"(\d+\.)*\d+").ToString());
        }
    }

    public Asset[]? Assets { get; init; }
}
