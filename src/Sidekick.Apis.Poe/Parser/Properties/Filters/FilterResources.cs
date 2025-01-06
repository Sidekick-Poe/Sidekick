using Microsoft.Extensions.Localization;

namespace Sidekick.Apis.Poe.Parser.Properties.Filters
{
    public class FilterResources(IStringLocalizer<FilterResources> localizer)
    {
        public string Filters_Damage => localizer["Damage"];
        public string Filters_Dps => localizer["Dps"];
        public string Filters_EDps => localizer["ElementalDps"];
        public string Filters_PDps => localizer["PhysicalDps"];
        public string Filters_ChaosDps => localizer["ChaosDps"];
    }
}
