
namespace Sidekick.Game.Providers.Languages.Implementations;

public class GameLanguageRu : IGameLanguage
{
    public string Code => "ru";
    public string Label => "Russian";

    public string PoeTradeBaseUrl => "https://ru.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://ru.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://ru.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://ru.pathofexile.com/api/trade2/";

    public string DescriptionQualityCurrency => "Качество (валюта)";
    public string DescriptionQualityScarabs => "Качество (скарабеи)";
    public string DescriptionQualityCards => "Качество (гадальные карты)";
    public string DescriptionQualityPackSize => "Качество (размер группы)";
    public string DescriptionQualityRarity => "Качество (редкость)";
    public string DescriptionHeistLockpicking => "Требуется взлом (# уровень)";
    public string DescriptionHeistDemolition => "Требуется взрывное дело (# уровень)";
    public string DescriptionHeistAgility => "Требуется проворство (# уровень)";
    public string DescriptionHeistCounterThaumaturgy => "Требуется контрмагия (# уровень)";
    public string DescriptionHeistTrap => "Требуется разминирование (# уровень)";
    public string DescriptionHeistPerception => "Требуется восприятие (# уровень)";
    public string DescriptionHeistBruteForce => "Требуется грубая сила (# уровень)";
    public string DescriptionHeistDeception => "Требуется маскировка (# уровень)";
    public string DescriptionHeistEngineering => "Требуется инженерное дело (# уровень)";
    public string DescriptionHeistModerateValue => "средней ценности";
    public string DescriptionHeistHighValue => "ценный";
    public string DescriptionHeistPrecious => "драгоценный";
    public string DescriptionHeistPriceless => "бесценный";
}

