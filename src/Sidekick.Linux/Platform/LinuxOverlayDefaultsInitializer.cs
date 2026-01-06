using Microsoft.Extensions.Logging;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;

namespace Sidekick.Linux.Platform;

public sealed class LinuxOverlayDefaultsInitializer(
    ISettingsService settingsService,
    ILogger<LinuxOverlayDefaultsInitializer> logger) : IInitializableService
{
    private const string MigrationKey = "LinuxOverlayCloseKeysMigrated";

    public int Priority => -100;

    public async Task Initialize()
    {
        if (!OperatingSystem.IsLinux())
        {
            return;
        }

        if (await settingsService.GetBool(MigrationKey))
        {
            return;
        }

        var keyClose = await settingsService.GetString(SettingKeys.KeyClose);
        if (!string.IsNullOrWhiteSpace(keyClose))
        {
            await settingsService.Set(SettingKeys.KeyClose, string.Empty);
            logger.LogInformation("[Linux] Cleared KeyClose binding for overlay.");
        }

        if (await settingsService.GetBool(SettingKeys.EscapeClosesOverlays))
        {
            await settingsService.Set(SettingKeys.EscapeClosesOverlays, false);
            logger.LogInformation("[Linux] Disabled EscapeClosesOverlays default.");
        }

        await settingsService.Set(MigrationKey, true);
    }
}
