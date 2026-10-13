
namespace Sidekick.Game.Providers.Languages.Implementations;

public class GameLanguagePt : IGameLanguage
{
    public string Code => "pt";
    public string Label => "Portuguese";

    public string PoeTradeBaseUrl => "https://br.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://br.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://br.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://br.pathofexile.com/api/trade2/";

    public string DescriptionQualityCurrency => "Qualidade (Moedas)";
    public string DescriptionQualityScarabs => "Qualidade (Escaravelhos)";
    public string DescriptionQualityCards => "Qualidade (Cartas de Adivinhação)";
    public string DescriptionQualityPackSize => "Qualidade (Tamanho de Grupos)";
    public string DescriptionQualityRarity => "Qualidade (Raridade)";
    public string DescriptionHeistLockpicking => "Requer Arrombamento (Nível #)";
    public string DescriptionHeistDemolition => "Requer Demolição (Nível #)";
    public string DescriptionHeistAgility => "Requer Agilidade (Nível #)";
    public string DescriptionHeistCounterThaumaturgy => "Requer Contra-Taumaturgia (Nível #)";
    public string DescriptionHeistTrap => "Requer Desarme de Armadilhas (Nível #)";
    public string DescriptionHeistPerception => "Requer Percepção (Nível #)";
    public string DescriptionHeistBruteForce => "Requer Força Bruta (Nível #)";
    public string DescriptionHeistDeception => "Requer Enganação (Nível #)";
    public string DescriptionHeistEngineering => "Requer Engenharia (Nível #)";
    public string DescriptionHeistModerateValue => "Valor Moderado";
    public string DescriptionHeistHighValue => "Valor Alto";
    public string DescriptionHeistPrecious => "Precioso";
    public string DescriptionHeistPriceless => "Sem Preço";
}

