using Sidekick.Apis.GitHub.Models;

namespace Sidekick.Apis.GitHub;

/// <summary>
///     Interface to communicate with GitHub.
/// </summary>
public interface IGitHubClient
{
    /// <summary>
    ///     Gets the latest release.
    /// </summary>
    /// <returns>True if an update is available.</returns>
    Task<GitHubRelease> GetLatestRelease();
}
