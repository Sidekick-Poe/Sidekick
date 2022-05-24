using Microsoft.Extensions.Localization;

namespace Sidekick.Modules.Maps.Localization
{
    public class MapInfoResources
    {
        private readonly IStringLocalizer<MapInfoResources> localizer;

        public MapInfoResources(IStringLocalizer<MapInfoResources> localizer)
        {
            this.localizer = localizer;
        }

        public string Title => localizer["Title"];
        public string Is_Safe => localizer["Is_Safe"];
        public string Is_Unsafe => localizer["Is_Unsafe"];
        public string PoeWikiTitle => localizer["PoeWikiTitle"];
        public string DropsTitle => localizer["DropsTitle"];
        public string PoeWikiError => localizer["PoeWikiError"];
        public string OpenInPoeWiki => localizer["OpenInPoeWiki"];
        public string NoDropsTitle => localizer["NoDropsTitle"];
        public string MapBossesTitle => localizer["MapBossesTitle"];
    }
}
