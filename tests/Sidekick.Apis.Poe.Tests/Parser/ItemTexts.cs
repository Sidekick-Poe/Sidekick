using Microsoft.Extensions.Localization;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    public class ItemTexts
    {
        private readonly IStringLocalizer<ItemTexts> localizer;

        public ItemTexts(IStringLocalizer<ItemTexts> localizer)
        {
            this.localizer = localizer;
        }

        public string ChaosOrb => localizer["ChaosOrb"];
        public string ArcardeMap => localizer["ArcardeMap"];
        public string ScreamingEssenceOfWoe => localizer["ScreamingEssenceOfWoe"];
        public string SacrificeAtMidnight => localizer["SacrificeAtMidnight"];
        public string RitualSplinter => localizer["RitualSplinter"];
        public string TimelessEternalEmpireSplinter => localizer["TimelessEternalEmpireSplinter"];
        public string DivineVessel => localizer["DivineVessel"];
        public string TributeToTheGoddess => localizer["TributeToTheGoddess"];
        public string SplinterOfTul => localizer["SplinterOfTul"];
        public string RustedReliquaryScarab => localizer["RustedReliquaryScarab"];
        public string BoonOfJustice => localizer["BoonOfJustice"];
        public string DaressosDefiance => localizer["DaressosDefiance"];
        public string ClearOil => localizer["ClearOil"];
        public string BlightedSpiderLairMap => localizer["BlightedSpiderLairMap"];
        public string SimulacrumSplinter => localizer["SimulacrumSplinter"];
        public string PerfectFossil => localizer["PerfectFossil"];
        public string PowerfulChaoticResonator => localizer["PowerfulChaoticResonator"];
        public string NoxiousCatalyst => localizer["NoxiousCatalyst"];
        public string BloodProgenitorsBrain => localizer["BloodProgenitorsBrain"];
        public string HallowedLifeFlask => localizer["HallowedLifeFlask"];
        public string HallowedManaFlask => localizer["HallowedManaFlask"];
        public string ArcaneSurgeSupport => localizer["ArcaneSurgeSupport"];
        public string VoidSphere => localizer["VoidSphere"];
        public string SacredHybridFlask => localizer["SacredHybridFlask"];
        public string ViridianJewel => localizer["ViridianJewel"];
        public string SmallClusterJewel => localizer["SmallClusterJewel"];
        public string InscribedUltimatum => localizer["InscribedUltimatum"];
        public string HeistTool => localizer["HeistTool"];
        public string Reefbane => localizer["Reefbane"];
        public string FarricChieftain => localizer["FarricChieftain"];
        public string HeistBrooch => localizer["HeistBrooch"];
        public string HeistCloak => localizer["HeistCloak"];
        public string HeistGear => localizer["HeistGear"];
        public string HeistTarget => localizer["HeistTarget"];
        public string ThiefTrinket => localizer["ThiefTrinket"];
        public string AbyssJewel => localizer["AbyssJewel"];
    }
}
