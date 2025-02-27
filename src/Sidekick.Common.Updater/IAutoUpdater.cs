using Velopack;

namespace Sidekick.Common.Updater;

/// <summary>
/// Provides functionality to check for, download, and apply updates.
/// </summary>
public interface IAutoUpdater
{
    bool IsUpdaterInstalled();

    /// <summary>
    /// Checks for available updates for the application.
    /// </summary>
    /// <returns>
    /// An <see cref="UpdateInfo"/> object if an update is available; otherwise, null.
    /// </returns>
    Task<UpdateInfo?> CheckForUpdates();

    /// <summary>
    /// Downloads and applies the provided update, then restarts the application.
    /// </summary>
    /// <param name="updateInfo">The information about the update to be downloaded and applied.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task UpdateAndRestart(UpdateInfo updateInfo);
}
