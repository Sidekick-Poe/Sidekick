using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Exceptions;
using Velopack;
using Velopack.Locators;

namespace Sidekick.Common.Updater;

public class AutoUpdater : IAutoUpdater
{
    private readonly ILogger<AutoUpdater> logger;

    private UpdateManager Manager { get; }

    public AutoUpdater(ILogger<AutoUpdater> logger)
    {
        this.logger = logger;
        var source = new Velopack.Sources.GithubSource("https://github.com/Sidekick-Poe/Sidekick", null, true);

        IVelopackLocator? locator = null;
        if (Debugger.IsAttached)
        {
            locator = new DebugLocator();
        }

        // We are retiring the windows-beta branch to simplify maintenance. Maintaining one version is easier than two.
        UpdateOptions? options = null;
        if (VelopackLocator.GetDefault(logger).Channel == "windows-beta")
        {
            options = new UpdateOptions
            {
                ExplicitChannel = "windows-stable",
                AllowVersionDowngrade = true,
            };
        }

        Manager = new UpdateManager(source, options, logger, locator);
    }

    public bool IsUpdaterInstalled()
    {
        try
        {
            return Manager.AppId is not null && Manager.IsInstalled;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[AutoUpdater] Error while checking if UpdateManager is installed.");
            throw new SidekickException("Could not check for updates", "Visit the website or the GitHub page to check manually.", "https://sidekick-poe.github.io/", "https://github.com/Sidekick-Poe/Sidekick/releases")
            {
                IsCritical = false,
            };
        }
    }

    public async Task<UpdateInfo?> CheckForUpdates()
    {
        if (!IsUpdaterInstalled())
        {
            logger.LogWarning("[AutoUpdater] UpdateManager is not installed.");
            return null;
        }

        try
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
        catch (Exception e)
        {
            logger.LogError(e, "[AutoUpdater] Error while checking for updates.");
            throw new SidekickException("Could not check for updates", "Visit the website or the GitHub page to check manually.", "https://sidekick-poe.github.io/", "https://github.com/Sidekick-Poe/Sidekick/releases")
            {
                IsCritical = false,
            };
        }
    }

    public async Task UpdateAndRestart(UpdateInfo updateInfo)
    {
        if (!IsUpdaterInstalled())
        {
            logger.LogWarning("[AutoUpdater] UpdateManager is not installed.");
            return;
        }

        try
        {
            // download new version
            logger.LogInformation("[AutoUpdater] Downloading updates.");
            await Manager.DownloadUpdatesAsync(updateInfo);

            // install new version and restart app
            logger.LogInformation("[AutoUpdater] Applying updates and restarting.");
            Manager.ApplyUpdatesAndRestart(updateInfo);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[AutoUpdater] Error while updating and restarting.");
            throw new SidekickException("Could not download the update automatically", "Visit the website or the GitHub page to download the latest version.", "https://sidekick-poe.github.io/", "https://github.com/Sidekick-Poe/Sidekick/releases")
            {
                IsCritical = false,
            };
        }
    }
}
