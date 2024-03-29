using Microsoft.Extensions.Localization;

namespace Sidekick.Modules.Settings.Localization
{
    public class SetupResources
    {
        private readonly IStringLocalizer<SetupResources> resources;

        public SetupResources(IStringLocalizer<SetupResources> resources)
        {
            this.resources = resources;
        }

        public string Exit => resources["Exit"];
        public string NewLeagues => resources["NewLeagues"];
        public string Title => resources["Title"];
    }
}
