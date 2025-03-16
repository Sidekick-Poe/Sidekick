namespace Sidekick.Common.Game.Languages;

public interface IGameLanguage
{
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

    string DescriptionUnidentified { get; }

    string DescriptionQuality { get; }

    string DescriptionAlternateQuality { get; }

    string DescriptionCorrupted { get; }

    string DescriptionSockets { get; }

    string DescriptionItemLevel { get; }

    string DescriptionMapTier { get; }

    string DescriptionReward { get; }

    string DescriptionAreaLevel { get; }

    string DescriptionItemQuantity { get; }

    string DescriptionItemRarity { get; }

    string DescriptionMonsterPackSize { get; }

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

    string DescriptionRequirements { get; }

    string AffixSuperior { get; }

    string AffixBlighted { get; }

    string AffixBlightRavaged { get; }

    string InfluenceShaper { get; }

    string InfluenceElder { get; }

    string InfluenceCrusader { get; }

    string InfluenceHunter { get; }

    string InfluenceRedeemer { get; }

    string InfluenceWarlord { get; }

    ClassLanguage Classes { get; }

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

    /// <summary>
    /// Applies a fuzzy transformation to the input text to match approximate text patterns.
    /// </summary>
    /// <param name="text">The input text to transform using a fuzzy logic mechanism.</param>
    /// <returns>Returns a transformed string based on fuzzy logic.</returns>
    string? GetFuzzyText(string? text) => text;
}
