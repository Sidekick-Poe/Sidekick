namespace Sidekick.Game.Providers.Languages;

public interface IGameLanguage
{
    string Code { get; }
    string Label { get; }

    string PoeTradeBaseUrl { get; }
    string PoeTradeApiBaseUrl { get; }
    string Poe2TradeBaseUrl { get; }
    string Poe2TradeApiBaseUrl { get; }

    string DescriptionQualityCurrency { get; }
    string DescriptionQualityScarabs { get; }
    string DescriptionQualityCards { get; }
    string DescriptionQualityPackSize { get; }
    string DescriptionQualityRarity { get; }
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
