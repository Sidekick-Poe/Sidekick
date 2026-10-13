
namespace Sidekick.Game.Providers.Languages.Implementations;

public class GameLanguageFr : IGameLanguage
{
    public string Code => "fr";
    public string Label => "French";

    public string PoeTradeBaseUrl => "https://fr.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://fr.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://fr.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://fr.pathofexile.com/api/trade2/";

    public string DescriptionPhysicalDamage => "Dégâts physiques";
    public string DescriptionElementalDamage => "Dégâts élémentaires";
    public string DescriptionFireDamage => "Dégâts de feu";
    public string DescriptionColdDamage => "Dégâts de froid";
    public string DescriptionLightningDamage => "Dégâts de foudre";
    public string DescriptionChaosDamage => "Dégâts de Chaos";
    public string DescriptionQualityCurrency => "Qualité (Objets monétaires)";
    public string DescriptionQualityScarabs => "Qualité (Scarabées)";
    public string DescriptionQualityCards => "Qualité (Cartes divinatoires)";
    public string DescriptionQualityPackSize => "Qualité (Taille des Groupes)";
    public string DescriptionQualityRarity => "Qualité (Rareté)";
    public string DescriptionHeistWings => "Ailes révélées";
    public string DescriptionHeistRoutes => "Échappatoires révélées";
    public string DescriptionHeistRooms => "Salles de récompenses révélées";
    public string DescriptionHeistLockpicking => "Prérequis : Crochetage au niv. #";
    public string DescriptionHeistDemolition => "Prérequis : Démolition au niv. #";
    public string DescriptionHeistAgility => "Prérequis : Agilité au niv. #";
    public string DescriptionHeistCounterThaumaturgy => "Prérequis : Anti-thaumaturgie au niv. #";
    public string DescriptionHeistTrap => "Prérequis : Désamorçage de pièges au niv. #";
    public string DescriptionHeistPerception => "Prérequis : Perception au niv. #";
    public string DescriptionHeistBruteForce => "Prérequis : Force brute au niv. #";
    public string DescriptionHeistDeception => "Prérequis : Duperie au niv. #";
    public string DescriptionHeistEngineering => "Prérequis : Ingénierie au niv. #";
    public string DescriptionHeistModerateValue => "valeur : moyenne";
    public string DescriptionHeistHighValue => "valeur : élevée";
    public string DescriptionHeistPrecious => "valeur : importante";
    public string DescriptionHeistPriceless => "valeur : inestimable";
}

