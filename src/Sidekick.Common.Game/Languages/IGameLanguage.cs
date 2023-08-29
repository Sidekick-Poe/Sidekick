namespace Sidekick.Common.Game.Languages
{
    public interface IGameLanguage
    {
        string LanguageCode { get; }

        Uri PoeTradeSearchBaseUrl { get; }
        Uri PoeTradeExchangeBaseUrl { get; }
        Uri PoeTradeApiBaseUrl { get; }
        Uri PoeCdnBaseUrl { get; }

        string RarityUnique { get; }
        string RarityRare { get; }
        string RarityMagic { get; }
        string RarityNormal { get; }
        string RarityCurrency { get; }
        string RarityGem { get; }
        string RarityDivinationCard { get; }

        string DescriptionUnidentified { get; }
        string DescriptionQuality { get; }
        string DescriptionAlternateQuality { get; }
        string DescriptionIsRelic { get; }
        string DescriptionCorrupted { get; }
        string DescriptionScourged { get; }
        string DescriptionSockets { get; }
        string DescriptionItemLevel { get; }
        string DescriptionMapTier { get; }
        string DescriptionItemQuantity { get; }
        string DescriptionItemRarity { get; }
        string DescriptionMonsterPackSize { get; }
        string DescriptionExperience { get; }
        string DescriptionPhysicalDamage { get; }
        string DescriptionElementalDamage { get; }
        string DescriptionAttacksPerSecond { get; }
        string DescriptionCriticalStrikeChance { get; }
        string DescriptionEnergyShield { get; }
        string DescriptionArmour { get; }
        string DescriptionEvasion { get; }
        string DescriptionChanceToBlock { get; }
        string DescriptionLevel { get; }
        string DescriptionRequirements { get; }

        string PrefixSuperior { get; }
        string PrefixBlighted { get; }
        string PrefixBlightRavaged { get; }
        string PrefixAnomalous { get; }
        string PrefixDivergent { get; }
        string PrefixPhantasmal { get; }

        string InfluenceShaper { get; }
        string InfluenceElder { get; }
        string InfluenceCrusader { get; }
        string InfluenceHunter { get; }
        string InfluenceRedeemer { get; }
        string InfluenceWarlord { get; }

        ClassLanguage? Classes { get; }
    }
}
