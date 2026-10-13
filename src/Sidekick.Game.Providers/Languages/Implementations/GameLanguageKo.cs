
namespace Sidekick.Game.Providers.Languages.Implementations;

public class GameLanguageKo : IGameLanguage
{
    public string Code => "ko";
    public string Label => "Korean";

    public string PoeTradeBaseUrl => "https://poe.kakaogames.com/trade/";
    public string PoeTradeApiBaseUrl => "https://poe.kakaogames.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://poe.kakaogames.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://poe.kakaogames.com/api/trade2/";

    public string DescriptionQualityCurrency => "퀄리티 (화폐)";
    public string DescriptionQualityScarabs => "퀄리티 (갑충석)";
    public string DescriptionQualityCards => "퀄리티 (점술 카드)";
    public string DescriptionQualityPackSize => "퀄리티 (무리 규모)";
    public string DescriptionQualityRarity => "퀄리티 (희귀도)";
    public string DescriptionHeistLockpicking => "자물쇠 따기 필요(#레벨)";
    public string DescriptionHeistDemolition => "파괴 공작 필요(#레벨)";
    public string DescriptionHeistAgility => "민첩함 필요(#레벨)";
    public string DescriptionHeistCounterThaumaturgy => "반-마석학 필요(#레벨)";
    public string DescriptionHeistTrap => "덫 해제 필요(#레벨)";
    public string DescriptionHeistPerception => "통찰력 필요(#레벨)";
    public string DescriptionHeistBruteForce => "완력 필요(#레벨)";
    public string DescriptionHeistDeception => "기만 필요(#레벨)";
    public string DescriptionHeistEngineering => "공학 필요(#레벨)";
    public string DescriptionHeistModerateValue => "보통 가치";
    public string DescriptionHeistHighValue => "높은 가치";
    public string DescriptionHeistPrecious => "귀중한 가치";
    public string DescriptionHeistPriceless => "환산 불가능한 가치";
}

