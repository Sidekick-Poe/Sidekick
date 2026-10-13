
namespace Sidekick.Game.Providers.Languages.Implementations;

public class GameLanguageDe : IGameLanguage
{
    public string Code => "de";
    public string Label => "German";

    public string PoeTradeBaseUrl => "https://de.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://de.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://de.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://de.pathofexile.com/api/trade2/";

    public string DescriptionPhysicalDamage => "Physischer Schaden";
    public string DescriptionElementalDamage => "Elementarschaden";
    public string DescriptionFireDamage => "Feuerschaden";
    public string DescriptionColdDamage => "Kälteschaden";
    public string DescriptionLightningDamage => "Blitzschaden";
    public string DescriptionChaosDamage => "Chaosschaden";
    public string DescriptionQualityCurrency => "Qualität (Währung)";
    public string DescriptionQualityScarabs => "Qualität (Skarabäen)";
    public string DescriptionQualityCards => "Qualität (Weissagungskarten)";
    public string DescriptionQualityPackSize => "Qualität (Gruppengröße)";
    public string DescriptionQualityRarity => "Qualität (Seltenheit)";
    public string DescriptionHeistWings => "Aufgedeckte Gebäudetrakte";
    public string DescriptionHeistRoutes => "Aufgedeckte Fluchtwege";
    public string DescriptionHeistRooms => "Aufgedeckte Belohnungsräume";
    public string DescriptionHeistLockpicking => "Erfordert Schlossknacken (Stufe #)";
    public string DescriptionHeistDemolition => "Erfordert Sprengung (Stufe #)";
    public string DescriptionHeistAgility => "Erfordert Agilität (Stufe #)";
    public string DescriptionHeistCounterThaumaturgy => "Erfordert Thaumaturgie-Abwehr (Stufe #)";
    public string DescriptionHeistTrap => "Erfordert Fallenentschärfung (Stufe #)";
    public string DescriptionHeistPerception => "Erfordert Wahrnehmung (Stufe #)";
    public string DescriptionHeistBruteForce => "Erfordert Brachialgewalt (Stufe #)";
    public string DescriptionHeistDeception => "Erfordert Täuschung (Stufe #)";
    public string DescriptionHeistEngineering => "Erfordert Ingenieurwesen (Stufe #)";
    public string DescriptionHeistModerateValue => "Mäßiger Wert";
    public string DescriptionHeistHighValue => "Hoher Wert";
    public string DescriptionHeistPrecious => "Kostbar";
    public string DescriptionHeistPriceless => "Unschätzbar";
}

