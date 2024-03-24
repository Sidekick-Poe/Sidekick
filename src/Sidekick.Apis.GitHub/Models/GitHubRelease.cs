namespace Sidekick.Apis.GitHub.Models;

public record GitHubRelease
{
    public bool IsNewerVersion { get; init; }

    public bool IsExecutable { get; init; }
}
