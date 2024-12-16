using Microsoft.Extensions.Localization;

namespace Sidekick.Apis.Poe.Localization
{
    public class FilterResources(IStringLocalizer<FilterResources> localizer)
    {
        public string Filters_Damage => localizer["Filters_Damage"];
        public string Filters_Dps => localizer["Filters_Dps"];
        public string Filters_EDps => localizer["Filters_EDps"];
        public string Filters_PDps => localizer["Filters_PDps"];
        public string Filters_ChaosDps => localizer["Filters_ChaosDps"];
    }
}
