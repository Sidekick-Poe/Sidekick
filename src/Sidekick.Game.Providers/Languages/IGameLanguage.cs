namespace Sidekick.Game.Providers.Languages;

public interface IGameLanguage
{
    string Code { get; }
    string Label { get; }

    string PoeTradeBaseUrl { get; }
    string PoeTradeApiBaseUrl { get; }
    string Poe2TradeBaseUrl { get; }
    string Poe2TradeApiBaseUrl { get; }

    string DescriptionUnidentified { get; }
    string DescriptionQuality { get; }
    string DescriptionCorrupted { get; }
    string DescriptionSplit { get; }
    string DescriptionMirrored { get; }
    string DescriptionSockets { get; }
    string DescriptionItemLevel { get; }
    string DescriptionMapTier { get; }
    string DescriptionReward { get; }
    string DescriptionAreaLevel { get; }
    string DescriptionItemQuantity { get; }
    string DescriptionMoreMaps { get; }
    string DescriptionMoreScarabs { get; }
    string DescriptionMoreCurrency { get; }
    string DescriptionMoreCards { get; }
    string DescriptionQualityCurrency { get; }
    string DescriptionQualityScarabs { get; }
    string DescriptionQualityCards { get; }
    string DescriptionQualityPackSize { get; }
    string DescriptionQualityRarity { get; }
    string DescriptionPhysicalDamage { get; }
    string DescriptionElementalDamage { get; }
    string DescriptionFireDamage { get; }
    string DescriptionColdDamage { get; }
    string DescriptionLightningDamage { get; }
    string DescriptionChaosDamage { get; }
    string DescriptionEnergyShield { get; }
    string DescriptionArmour { get; }
    string DescriptionEvasion { get; }
    string DescriptionSpirit { get; }
    string DescriptionLevel { get; }
    string DescriptionMemoryStrands { get; }
    string DescriptionRequirements { get; }
    string DescriptionRequires { get; }
    string DescriptionRequiresLevel { get; }
    string DescriptionRequiresStr { get; }
    string DescriptionRequiresDex { get; }
    string DescriptionRequiresInt { get; }
    string DescriptionHeistWings { get; }
    string DescriptionHeistRoutes { get; }
    string DescriptionHeistRooms { get; }
    string DescriptionHeistLockpicking { get; }
    string DescriptionHeistDemolition { get; }
    string DescriptionHeistAgility { get; }
    string DescriptionHeistCounterThaumaturgy { get; }
    string DescriptionHeistTrap { get; }
    string DescriptionHeistPerception { get; }
    string DescriptionHeistBruteForce { get; }
    string DescriptionHeistDeception { get; }
    string DescriptionHeistEngineering { get; }
    string DescriptionHeistModerateValue { get; }
    string DescriptionHeistHighValue { get; }
    string DescriptionHeistPrecious { get; }
    string DescriptionHeistPriceless { get; }

    public string GetTradeBaseUrl(GameType game) => game switch
    {
        GameType.Poe2 => Poe2TradeBaseUrl,
        _ => PoeTradeBaseUrl,
    };

    public string GetTradeApiBaseUrl(GameType game) => game switch
    {
        GameType.Poe2 => Poe2TradeApiBaseUrl,
        _ => PoeTradeApiBaseUrl,
    };
}
