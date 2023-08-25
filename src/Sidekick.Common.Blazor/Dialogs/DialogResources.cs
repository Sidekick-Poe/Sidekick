using Microsoft.Extensions.Localization;

namespace Sidekick.Common.Blazor.Dialogs
{
    public class DialogResources
    {
        private readonly IStringLocalizer<DialogResources> localizer;

        public DialogResources(IStringLocalizer<DialogResources> localizer)
        {
            this.localizer = localizer;
        }

        public string No => localizer["No"];
        public string Title => localizer["Title"];
        public string Yes => localizer["Yes"];
    }
}
