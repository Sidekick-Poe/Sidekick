using Microsoft.Extensions.Localization;

namespace Sidekick.Common.Blazor.Initialization
{
    public class InitializationResources
    {
        private readonly IStringLocalizer<InitializationResources> localizer;

        public InitializationResources(IStringLocalizer<InitializationResources> localizer)
        {
            this.localizer = localizer;
        }

        public string Error => localizer["Error"];
        public string Notification => localizer["Notification"];
        public string Ready => localizer["Ready"];

        public string Title(int completed, int count) => localizer["Title", completed, count];

        public string UpdateAvailable => localizer["UpdateAvailable"];
        public string UpdateTitle => localizer["UpdateTitle"];
        public string Exit => localizer["Exit"];
        public string Close => localizer["Close"];
    }
}
