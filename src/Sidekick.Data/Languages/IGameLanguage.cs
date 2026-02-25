using Sidekick.Data.Items.Models;
namespace Sidekick.Data.Languages;

public interface IGameLanguage
{
    string Code { get; }

    string Label { get; }

    string PoeTradeBaseUrl { get; }

    string PoeTradeApiBaseUrl { get; }

    string Poe2TradeBaseUrl { get; }

    string Poe2TradeApiBaseUrl { get; }

    Uri PoeCdnBaseUrl { get; }

    string RarityUnique { get; }

    string RarityRare { get; }

    string RarityMagic { get; }

    string RarityNormal { get; }

    string RarityCurrency { get; }

    string RarityGem { get; }

    string RarityDivinationCard { get; }

    string DescriptionRarity { get; }

    string DescriptionUnidentified { get; }

    string DescriptionQuality { get; }

    string DescriptionCorrupted { get; }

    string DescriptionSockets { get; }

    string DescriptionItemLevel { get; }

    string DescriptionMapTier { get; }

    string DescriptionReward { get; }

    string DescriptionAreaLevel { get; }

    string DescriptionItemQuantity { get; }

    string DescriptionItemRarity { get; }

    string DescriptionMonsterPackSize { get; }

    string DescriptionMagicMonsters { get; }

    string DescriptionRareMonsters { get; }

    string DescriptionRevivesAvailable { get; }

    string DescriptionWaystoneDropChance { get; }

    string DescriptionExperience { get; }

    string DescriptionPhysicalDamage { get; }

    string DescriptionElementalDamage { get; }

    string DescriptionFireDamage { get; }

    string DescriptionColdDamage { get; }

    string DescriptionLightningDamage { get; }

    string DescriptionChaosDamage { get; }

    string DescriptionAttacksPerSecond { get; }

    string DescriptionCriticalStrikeChance { get; }

    string DescriptionCriticalHitChance { get; }

    string DescriptionEnergyShield { get; }

    string DescriptionEnergyShieldAlternate { get; }

    string DescriptionArmour { get; }

    string DescriptionEvasion { get; }

    string DescriptionChanceToBlock { get; }

    string DescriptionBlockChance { get; }

    string DescriptionSpirit { get; }

    string DescriptionLevel { get; }

    string DescriptionUnusable { get; }

    string DescriptionRequirements { get; }

    string DescriptionRequires { get; }

    string DescriptionRequiresLevel { get; }

    string DescriptionRequiresStr { get; }

    string DescriptionRequiresDex { get; }

    string DescriptionRequiresInt { get; }

    string AffixSuperior { get; }

    string AffixBlighted { get; }

    string AffixBlightRavaged { get; }

    string InfluenceShaper { get; }

    string InfluenceElder { get; }

    string InfluenceCrusader { get; }

    string InfluenceHunter { get; }

    string InfluenceRedeemer { get; }

    string InfluenceWarlord { get; }

    string RegexIncreased { get; }

    string RegexReduced { get; }

    string RegexMore { get; }

    string RegexLess { get; }

    string RegexFaster { get; }

    string RegexSlower { get; }

    string ClassPrefix { get; }
    string ClassDivinationCard { get; }
    string ClassStackableCurrency { get; }
    string ClassJewel { get; }
    string ClassDelveStackableSocketableCurrency { get; }
    string ClassHeistTool { get; }
    string ClassAmulet { get; }
    string ClassRing { get; }
    string ClassBelt { get; }
    string ClassGloves { get; }
    string ClassBoots { get; }
    string ClassBodyArmours { get; }
    string ClassHelmets { get; }
    string ClassShields { get; }
    string ClassQuivers { get; }
    string ClassLifeFlasks { get; }
    string ClassManaFlasks { get; }
    string ClassHybridFlasks { get; }
    string ClassUtilityFlasks { get; }
    string ClassActiveSkillGems { get; }
    string ClassSupportSkillGems { get; }
    string ClassUncutSkillGems { get; }
    string ClassUncutSupportGems { get; }
    string ClassUncutSpiritGems { get; }
    string ClassMaps { get; }
    string ClassMapFragments { get; }
    string ClassContract { get; }
    string ClassBlueprint { get; }
    string ClassMiscMapItems { get; }
    string ClassClaws { get; }
    string ClassDaggers { get; }
    string ClassWands { get; }
    string ClassOneHandSwords { get; }
    string ClassThrustingOneHandSwords { get; }
    string ClassOneHandAxes { get; }
    string ClassOneHandMaces { get; }
    string ClassBows { get; }
    string ClassStaves { get; }
    string ClassTwoHandSwords { get; }
    string ClassTwoHandAxes { get; }
    string ClassTwoHandMaces { get; }
    string ClassSceptres { get; }
    string ClassRuneDaggers { get; }
    string ClassWarstaves { get; }
    string ClassQuarterstaves { get; }
    string ClassFishingRods { get; }
    string ClassHeistGear { get; }
    string ClassHeistBrooch { get; }
    string ClassHeistTarget { get; }
    string ClassHeistCloak { get; }
    string ClassAbyssJewel { get; }
    string ClassTrinkets { get; }
    string ClassLogbooks { get; }
    string ClassMemoryLine { get; }
    string ClassSanctumResearch { get; }
    string ClassSanctumRelics { get; }
    string ClassTinctures { get; }
    string ClassCorpses { get; }
    string ClassSocketable { get; }
    string ClassFocus { get; }
    string ClassWaystone { get; }
    string ClassBarya { get; }
    string ClassUltimatum { get; }
    string ClassTablet { get; }
    string ClassCrossbows { get; }
    string ClassOmen { get; }
    string ClassBreachstone { get; }
    string ClassBossKey { get; }
    string ClassSpears { get; }
    string ClassBucklers { get; }
    string ClassGrafts { get; }
    string ClassWombgifts { get; }
    string ClassTalismans { get; }
    string ClassCharms { get; }

    public string GetTradeBaseUrl(GameType game) => game switch
    {
        GameType.PathOfExile2 => Poe2TradeBaseUrl,
        _ => PoeTradeBaseUrl,
    };

    public string GetTradeApiBaseUrl(GameType game) => game switch
    {
        GameType.PathOfExile2 => Poe2TradeApiBaseUrl,
        _ => PoeTradeApiBaseUrl,
    };
}
