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

    /// <summary>
    ///     Downloads the latest release from github.
    /// </summary>
    /// <param name="downloadPath">The path where the file is to be downloaded.</param>
    /// <returns>True if the file downloaded successfully.</returns>
    Task<bool> DownloadLatest(string downloadPath);
}
