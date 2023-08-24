using System.Threading.Tasks;

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
        /// <returns>The path where the file was downloaded.</returns>
        Task<string> DownloadLatest();
    }
}
