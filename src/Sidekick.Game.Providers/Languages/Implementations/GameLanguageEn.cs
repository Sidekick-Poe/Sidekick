
namespace Sidekick.Game.Providers.Languages.Implementations;

public class GameLanguageEn : IGameLanguage
{
    public string Code => "en";
    public string Label => "English";

    public string PoeTradeBaseUrl => "https://www.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://www.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://www.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://www.pathofexile.com/api/trade2/";

    public string DescriptionQualityCurrency => "Quality (Currency)";
    public string DescriptionQualityScarabs => "Quality (Scarabs)";
    public string DescriptionQualityCards => "Quality (Divination Cards)";
    public string DescriptionQualityPackSize => "Quality (Pack Size)";
    public string DescriptionQualityRarity => "Quality (Rarity)";
    public string DescriptionHeistWings => "Wings Revealed";
    public string DescriptionHeistRoutes => "Escape Routes Revealed";
    public string DescriptionHeistRooms => "Reward Rooms Revealed";
    public string DescriptionHeistLockpicking => "Requires Lockpicking (Level #)";
    public string DescriptionHeistDemolition => "Requires Demolition (Level #)";
    public string DescriptionHeistAgility => "Requires Agility (Level #)";
    public string DescriptionHeistCounterThaumaturgy => "Requires Counter-Thaumaturgy (Level #)";
    public string DescriptionHeistTrap => "Requires Trap Disarmament (Level #)";
    public string DescriptionHeistPerception => "Requires Perception (Level #)";
    public string DescriptionHeistBruteForce => "Requires Brute Force (Level #)";
    public string DescriptionHeistDeception => "Requires Deception (Level #)";
    public string DescriptionHeistEngineering => "Requires Engineering (Level #)";
    public string DescriptionHeistModerateValue => "Moderate Value";
    public string DescriptionHeistHighValue => "High Value";
    public string DescriptionHeistPrecious => "Precious";
    public string DescriptionHeistPriceless => "Priceless";
}

