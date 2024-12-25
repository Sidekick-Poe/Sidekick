using Microsoft.Extensions.Logging;
using Velopack;

namespace Sidekick.Common.Updater;

public class AutoUpdater(
    ILogger<AutoUpdater> logger) : IAutoUpdater
{
    public async Task CheckForUpdates()
    {
        var source = new Velopack.Sources.GithubSource("https://github.com/Sidekick-Poe/Sidekick", null, false);
        var updateManager = new UpdateManager(source);

        logger.LogInformation("[AutoUpdater] Checking for updates.");
        var newVersion = await updateManager.CheckForUpdatesAsync();
        if (newVersion == null)
        {
            logger.LogInformation("[AutoUpdater] No updates found.");
            return;
        }

        // download new version
        logger.LogInformation("[AutoUpdater] Downloading updates.");
        await updateManager.DownloadUpdatesAsync(newVersion);

        // install new version and restart app
        logger.LogInformation("[AutoUpdater] Applying updates and restarting.");
        updateManager.ApplyUpdatesAndRestart(newVersion);
    }
}
