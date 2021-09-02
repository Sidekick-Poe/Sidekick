using Microsoft.Extensions.Localization;

namespace Sidekick.Modules.Cheatsheets.Localization
{
    public class BlightResources
    {
        private readonly IStringLocalizer<BlightResources> localizer;

        public BlightResources(IStringLocalizer<BlightResources> localizer)
        {
            this.localizer = localizer;
        }

        public string AmberOil => localizer["AmberOil"];
        public string AmberOilEffect => localizer["AmberOilEffect"];
        public string AzureOil => localizer["AzureOil"];
        public string AzureOilEffect => localizer["AzureOilEffect"];
        public string BlackOil => localizer["BlackOil"];
        public string BlackOilEffect => localizer["BlackOilEffect"];
        public string ClearOil => localizer["ClearOil"];
        public string ClearOilEffect => localizer["ClearOilEffect"];
        public string CrimsonOil => localizer["CrimsonOil"];
        public string CrimsonOilEffect => localizer["CrimsonOilEffect"];
        public string GoldenOil => localizer["GoldenOil"];
        public string GoldenOilEffect => localizer["GoldenOilEffect"];
        public string IndigoOil => localizer["IndigoOil"];
        public string IndigoOilEffect => localizer["IndigoOilEffect"];
        public string OpalescentOil => localizer["OpalescentOil"];
        public string OpalescentOilEffect => localizer["OpalescentOilEffect"];
        public string SepiaOil => localizer["SepiaOil"];
        public string SepiaOilEffect => localizer["SepiaOilEffect"];
        public string SilverOil => localizer["SilverOil"];
        public string SilverOilEffect => localizer["SilverOilEffect"];
        public string TealOil => localizer["TealOil"];
        public string TealOilEffect => localizer["TealOilEffect"];
        public string VerdantOil => localizer["VerdantOil"];
        public string VerdantOilEffect => localizer["VerdantOilEffect"];
        public string VioletOil => localizer["VioletOil"];
        public string VioletOilEffect => localizer["VioletOilEffect"];
    }
}
