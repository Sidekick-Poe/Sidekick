using Microsoft.Extensions.Localization;

namespace Sidekick.Modules.Cheatsheets.Localization
{
    public class CheatsheetResources
    {
        private readonly IStringLocalizer<CheatsheetResources> localizer;

        public CheatsheetResources(IStringLocalizer<CheatsheetResources> localizer)
        {
            this.localizer = localizer;
        }

        public string Cheatsheets => localizer["Cheatsheets"];
        public string Pages => localizer["Pages"];
    }
}
