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
        public string Betrayal => localizer["Betrayal"];
        public string Delve => localizer["Delve"];
        public string Blight => localizer["Blight"];
        public string Heist => localizer["Heist"];
        public string Incursion => localizer["Incursion"];
        public string Metamorph => localizer["Metamorph"];
        public string Leagues => localizer["Leagues"];
        public string Value_Low => localizer["Value_Low"];
        public string Value_Medium => localizer["Value_Medium"];
        public string Value_None => localizer["Value_None"];
        public string Value_High => localizer["Value_High"];
        public string Archnemesis => localizer["Archnemesis"];
    }
}
