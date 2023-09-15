using Microsoft.Extensions.Localization;

namespace Sidekick.Apis.Poe.Localization
{
    public class TradeCurrencyResources
    {
        private readonly IStringLocalizer<TradeCurrencyResources> localizer;

        public TradeCurrencyResources(IStringLocalizer<TradeCurrencyResources> localizer)
        {
            this.localizer = localizer;
        }

        public string ChaosEquivalent => localizer["ChaosEquivalent"];
        public string ChaosOrDivine => localizer["ChaosOrDivine"];
        public string Exalted => localizer["Exalted"];
        public string Divine => localizer["Divine"];
        public string GoldCoin => localizer["GoldCoin"];
        public string Blessed => localizer["Blessed"];
        public string Cartographer => localizer["Cartographer"];
        public string Chromatic => localizer["Chromatic"];
        public string Gemcutter => localizer["Gemcutter"];
        public string Jeweller => localizer["Jeweller"];
        public string Scouring => localizer["Scouring"];
        public string Regret => localizer["Regret"];
        public string Fusing => localizer["Fusing"];
        public string Chance => localizer["Chance"];
        public string Alteration => localizer["Alteration"];
        public string Alchemy => localizer["Alchemy"];
        public string Regal => localizer["Regal"];
        public string Vaal => localizer["Vaal"];
    }
}
