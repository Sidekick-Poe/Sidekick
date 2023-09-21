using Microsoft.Extensions.Localization;

namespace Sidekick.Modules.About.Localization
{
    public class AboutResources
    {
        private readonly IStringLocalizer<AboutResources> localizer;

        public AboutResources(IStringLocalizer<AboutResources> localizer)
        {
            this.localizer = localizer;
        }

        public string Title => localizer["Title"];
        public string Operating_System => localizer["Operating_System"];
        public string Environment_Versions => localizer["Environment_Versions"];
        public string Project_Page => localizer["Project_Page"];
        public string Bug_Reports => localizer["Bug_Reports"];
        public string Contributors => localizer["Contributors"];
        public string Translators => localizer["Translators"];
        public string Third_Parties => localizer["Third_Parties"];
        public string Disclaimer => localizer["Disclaimer"];
    }
}
