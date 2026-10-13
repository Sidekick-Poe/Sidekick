
namespace Sidekick.Game.Providers.Languages.Implementations;

public class GameLanguageEs : IGameLanguage
{
    public string Code => "es";
    public string Label => "Spanish";

    public string PoeTradeBaseUrl => "https://es.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://es.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://es.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://es.pathofexile.com/api/trade2/";

    public string DescriptionQualityCurrency => "Calidad (objetos monetarios)";
    public string DescriptionQualityScarabs => "Calidad (escarabajos)";
    public string DescriptionQualityCards => "Calidad (cartas de adivinación)";
    public string DescriptionQualityPackSize => "Calidad (tamaño de grupos de monstruos)";
    public string DescriptionQualityRarity => "Calidad (rareza)";
    public string DescriptionHeistLockpicking => "Requiere Forzar cerraduras (Nivel#)";
    public string DescriptionHeistDemolition => "Requiere Demolición (Nivel#)";
    public string DescriptionHeistAgility => "Requiere Agilidad (Nivel#)";
    public string DescriptionHeistCounterThaumaturgy => "Requiere Antitaumaturgia (Nivel#)";
    public string DescriptionHeistTrap => "Requiere Desarme de trampas (Nivel#)";
    public string DescriptionHeistPerception => "Requiere Percepción (Nivel#)";
    public string DescriptionHeistBruteForce => "Requiere Fuerza bruta (Nivel#)";
    public string DescriptionHeistDeception => "Requiere Engaño (Nivel#)";
    public string DescriptionHeistEngineering => "Requiere Ingeniería (Nivel#)";
    public string DescriptionHeistModerateValue => "valor moderado";
    public string DescriptionHeistHighValue => "valor alto";
    public string DescriptionHeistPrecious => "valor muy alto";
    public string DescriptionHeistPriceless => "valor incalculable";
}

