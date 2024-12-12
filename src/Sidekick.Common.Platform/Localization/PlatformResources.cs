using Microsoft.Extensions.Localization;

namespace Sidekick.Common.Platform.Localization
{
    public class PlatformResources(IStringLocalizer<PlatformResources> localizer)
    {
        /// <summary>
        /// Error message for when the application is not run as administrator, but the game is.
        /// </summary>
        public string RestartAsAdminText => localizer["RestartAsAdminText"];
    }
}
