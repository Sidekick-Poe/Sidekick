using Microsoft.Extensions.Logging;
using Velopack;

namespace Sidekick.Apis.Velopack;

public class AutoUpdater(
    ILogger<AutoUpdater> logger) : IAutoUpdater
{
    public async Task CheckForUpdates()
    {
        var updateManager = new UpdateManager("https://sidekick-poe.github.io/Sidekick-Velopack/");

        logger.LogInformation("[AutoAupdater] Checking for updates.");
        var newVersion = await updateManager.CheckForUpdatesAsync();
        if (newVersion == null)
        {
            logger.LogInformation("[AutoAupdater] No updates found.");
            return;
        }

        // download new version
        logger.LogInformation("[AutoAupdater] Downloading updates.");
        await updateManager.DownloadUpdatesAsync(newVersion);

        // install new version and restart app
        logger.LogInformation("[AutoAupdater] Applying updates and restarting.");
        updateManager.ApplyUpdatesAndRestart(newVersion);
    }
}
