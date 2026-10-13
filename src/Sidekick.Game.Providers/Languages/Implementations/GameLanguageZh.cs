
namespace Sidekick.Game.Providers.Languages.Implementations;

public class GameLanguageZh : IGameLanguage
{
    public string Code => "zh";
    public string Label => "Traditional Chinese";

    public string PoeTradeBaseUrl => "http://www.pathofexile.tw/trade/";
    public string PoeTradeApiBaseUrl => "http://www.pathofexile.tw/api/trade/";
    public string Poe2TradeBaseUrl => "http://www.pathofexile.tw/trade2/";
    public string Poe2TradeApiBaseUrl => "http://www.pathofexile.tw/api/trade2/";

    public string DescriptionQualityCurrency => "";
    public string DescriptionQualityScarabs => "";
    public string DescriptionQualityCards => "";
    public string DescriptionQualityPackSize => "";
    public string DescriptionQualityRarity => "";
    public string DescriptionHeistLockpicking => "";
    public string DescriptionHeistDemolition => "";
    public string DescriptionHeistAgility => "";
    public string DescriptionHeistCounterThaumaturgy => "";
    public string DescriptionHeistTrap => "";
    public string DescriptionHeistPerception => "";
    public string DescriptionHeistBruteForce => "";
    public string DescriptionHeistDeception => "";
    public string DescriptionHeistEngineering => "";
    public string DescriptionHeistModerateValue => "";
    public string DescriptionHeistHighValue => "";
    public string DescriptionHeistPrecious => "";
    public string DescriptionHeistPriceless => "";
}

