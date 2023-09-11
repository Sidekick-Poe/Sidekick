namespace Sidekick.Apis.GitHub
{
    /// <summary>
    /// Interface to communicate with GitHub.
    /// </summary>
    public interface IGitHubClient
    {
        /// <summary>
        /// Determines if there is a newer version available.
        /// </summary>
        /// <returns>True if an update is available.</returns>
        Task<bool> IsUpdateAvailable();

        /// <summary>
        /// Downloads the latest release from github.
        /// </summary>
        /// <param name="downloadPath">The path where the file is to be downloaded.</param>
        /// <returns>True if the file downloaded successfully.</returns>
        Task<bool> DownloadLatest(string downloadPath);
    }
}
