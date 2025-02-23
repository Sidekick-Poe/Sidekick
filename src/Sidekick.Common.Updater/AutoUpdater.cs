using Microsoft.Extensions.Logging;
using Velopack;
using Velopack.Locators;

namespace Sidekick.Common.Updater;

public class AutoUpdater : IAutoUpdater
{
    private readonly ILogger<AutoUpdater> logger;

    public AutoUpdater(ILogger<AutoUpdater> logger)
    {
        this.logger = logger;
        var source = new Velopack.Sources.GithubSource("https://github.com/Sidekick-Poe/Sidekick", null, true);
        var options = new UpdateOptions { AllowVersionDowngrade = true, };

        IVelopackLocator? locator = null;
#if DEBUG
        locator = new DebugLocator();
#endif

        // We are retiring the windows-beta branch to simplify maintenance. Maintaining one version is easier than two.
        if (VelopackLocator.GetDefault(logger).Channel == "windows-beta")
        {
            options.ExplicitChannel = "windows-stable";
        }

        Manager = new UpdateManager(source, options, logger, locator);
    }

    private UpdateManager Manager { get; set; }

    public async Task<UpdateInfo?> CheckForUpdates()
    {
        logger.LogInformation("[AutoUpdater] Checking for updates.");
        var updateInfo = await Manager.CheckForUpdatesAsync();
        if (updateInfo == null)
        {
            logger.LogInformation("[AutoUpdater] No updates found.");
            return null;
        }

        logger.LogInformation("[AutoUpdater] Found a new update.");
        return updateInfo;
    }

    public async Task UpdateAndRestart(UpdateInfo updateInfo)
    {
        // download new version
        logger.LogInformation("[AutoUpdater] Downloading updates.");
        await Manager.DownloadUpdatesAsync(updateInfo);

        // install new version and restart app
        logger.LogInformation("[AutoUpdater] Applying updates and restarting.");
        Manager.ApplyUpdatesAndRestart(updateInfo);
    }
}
