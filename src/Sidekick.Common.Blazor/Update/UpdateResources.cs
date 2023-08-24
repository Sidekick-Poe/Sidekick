using Microsoft.Extensions.Localization;

namespace Sidekick.Common.Blazor.Update
{
    public class UpdateResources
    {
        private readonly IStringLocalizer<UpdateResources> localizer;

        public UpdateResources(IStringLocalizer<UpdateResources> localizer)
        {
            this.localizer = localizer;
        }

        public string Available => localizer["Available"];
        public string Checking => localizer["Checking"];
        public string Close => localizer["Close"];
        public string Confirm => localizer["Confirm"];
        public string Download => localizer["Download"];
        public string Downloaded => localizer["Downloaded"];
        public string Downloading => localizer["Downloading"];
        public string Failed => localizer["Failed"];
        public string NotAvaialble => localizer["NotAvaialble"];
        public string Title => localizer["Title"];
    }
}
