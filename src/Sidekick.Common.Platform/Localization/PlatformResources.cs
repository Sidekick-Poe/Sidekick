using Microsoft.Extensions.Localization;

namespace Sidekick.Common.Platforms.Localization
{
    public class PlatformResources
    {
        private readonly IStringLocalizer<PlatformResources> localizer;

        public PlatformResources(IStringLocalizer<PlatformResources> localizer)
        {
            this.localizer = localizer;
        }

        /// <summary>
        /// Error message for when the application is not run as administrator, but the game is.
        /// </summary>
        public string RestartAsAdminText => localizer["RestartText"];
    }
}
