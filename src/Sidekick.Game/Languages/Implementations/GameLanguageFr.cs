
namespace Sidekick.Game.Languages.Implementations;

public class GameLanguageFr : IGameLanguage
{
    public string Code => "fr";
    public string Label => "French";

    public string PoeTradeBaseUrl => "https://fr.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://fr.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://fr.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://fr.pathofexile.com/api/trade2/";

    public string RarityUnique => "Unique";
    public string RarityRare => "Rare";
    public string RarityMagic => "Magique";
    public string RarityNormal => "Normal";
    public string RarityCurrency => "Objet monétaire";
    public string RarityGem => "Gemme";
    public string RarityDivinationCard => "Carte divinatoire";

    public string DescriptionRarity => "Rareté";
    public string DescriptionUnidentified => "Non identifié";
    public string DescriptionQuality => "Qualité";
    public string DescriptionLevel => "Niveau";
    public string DescriptionCorrupted => "Corrompu";
    public string DescriptionSplit => "Scindé";
    public string DescriptionMirrored => "Reflété";
    public string DescriptionSockets => "Châsses";
    public string DescriptionItemLevel => "Niveau de l'objet";
    public string DescriptionExperience => "Expérience";
    public string DescriptionPhysicalDamage => "Dégâts physiques";
    public string DescriptionElementalDamage => "Dégâts élémentaires";
    public string DescriptionFireDamage => "Dégâts de feu";
    public string DescriptionColdDamage => "Dégâts de froid";
    public string DescriptionLightningDamage => "Dégâts de foudre";
    public string DescriptionChaosDamage => "Dégâts de Chaos";
    public string DescriptionEnergyShield => "Bouclier d'énergie";
    public string DescriptionEnergyShieldAlternate => "";
    public string DescriptionArmour => "Armure";
    public string DescriptionEvasion => "Score d'Évasion";
    public string DescriptionChanceToBlock => "Chances de blocage";
    public string DescriptionBlockChance => "Chances de Blocage";
    public string DescriptionSpirit => "Esprit";
    public string DescriptionAttacksPerSecond => "Attaques par seconde";
    public string DescriptionCriticalStrikeChance => "Chances de coup critique";
    public string DescriptionCriticalHitChance => "Chances de Touche critique";
    public string DescriptionMapTier => "Palier de Carte";
    public string DescriptionReward => "Récompense";
    public string DescriptionItemQuantity => "Quantité d'objets";
    public string DescriptionItemRarity => "Rareté des objets";
    public string DescriptionMonsterPackSize => "Taille des groupes de monstres";
    public string DescriptionMoreMaps => "Davantage de Cartes";
    public string DescriptionMoreScarabs => "Davantage de Scarabées";
    public string DescriptionMoreCurrency => "Davantage d'Objets monétaires";
    public string DescriptionMoreCards => "Davantage de Cartes divinatoires";
    public string DescriptionQualityCurrency => "Qualité (Objets monétaires)";
    public string DescriptionQualityScarabs => "Qualité (Scarabées)";
    public string DescriptionQualityCards => "Qualité (Cartes divinatoires)";
    public string DescriptionQualityPackSize => "Qualité (Taille des Groupes)";
    public string DescriptionQualityRarity => "Qualité (Rareté)";
    public string DescriptionMagicMonsters => "Monstres magiques";
    public string DescriptionRareMonsters => "Monstres rares";
    public string DescriptionRevivesAvailable => "Résurrections disponibles";
    public string DescriptionWaystoneDropChance => "Chances de trouver des Pierres de téléportation";
    public string DescriptionAreaLevel => "Niveau de la zone";
    public string DescriptionMemoryStrands => "Brins de Souvenir";
    public string DescriptionUnusable => "Vous ne pouvez pas utiliser cet objet ; ses stats sont ignorées.";
    public string DescriptionRequirements => "Prérequis";
    public string DescriptionRequires => "Prérequis";
    public string DescriptionRequiresLevel => "Niveau";
    public string DescriptionRequiresStr => "For";
    public string DescriptionRequiresDex => "Dex";
    public string DescriptionRequiresInt => "Int";
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

    public string AffixSuperior => "supérieur";
    public string AffixBlighted => "Carte infestée";
    public string AffixBlightRavaged => "Carte ravagée par l'Infestation";
}

