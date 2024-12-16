namespace Sidekick.Common.Game.Languages.Implementations;

[GameLanguage("French", "fr")]
public class GameLanguageFR : IGameLanguage
{
    public string PoeTradeBaseUrl => "https://fr.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://fr.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://fr.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://fr.pathofexile.com/api/trade2/";
    public Uri PoeCdnBaseUrl => new("https://web.poecdn.com/");

    public string RarityUnique => "Unique";
    public string RarityRare => "Rare";
    public string RarityMagic => "Magique";
    public string RarityNormal => "Normal";
    public string RarityCurrency => "Objet monétaire";
    public string RarityGem => "Gemme";
    public string RarityDivinationCard => "Carte divinatoire";

    public string DescriptionUnidentified => "Non identifié";
    public string DescriptionQuality => "Qualité";
    public string DescriptionAlternateQuality => "Qualité alternative";
    public string DescriptionLevel => "Niveau";
    public string DescriptionCorrupted => "Corrompu";
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
    public string DescriptionArmour => "Armure";
    public string DescriptionEvasion => "Score d'Évasion";
    public string DescriptionChanceToBlock => "Chances de blocage";
    public string DescriptionAttacksPerSecond => "Attaques par seconde";
    public string DescriptionCriticalStrikeChance => "Chances de coup critique";
    public string DescriptionMapTier => "Palier de Carte";
    public string DescriptionItemQuantity => "Quantité d'objets";
    public string DescriptionItemRarity => "Rareté des objets";
    public string DescriptionMonsterPackSize => "Taille des groupes de monstres";
    public string DescriptionRequirements => "Prérequis";
    public string DescriptionAreaLevel => "Niveau de la zone";

    public string AffixSuperior => "supérieur";
    public string AffixBlighted => "Carte infestée";
    public string AffixBlightRavaged => "Carte ravagée par l'Infestation";
    public string AffixAnomalous => "/anormale?s?/";
    public string AffixDivergent => "/divergente?s?/";
    public string AffixPhantasmal => "/fantasmatiques?/";

    public string InfluenceShaper => "Objet du Façonneur";
    public string InfluenceElder => "Objet de l'Ancien";
    public string InfluenceCrusader => "Objet du Croisé";
    public string InfluenceHunter => "Objet du Chasseur";
    public string InfluenceRedeemer => "Objet de la Rédemptrice";
    public string InfluenceWarlord => "Objet du Seigneur de guerre";

    public ClassLanguage Classes { get; } = new()
    {
        Prefix = "Classe d'objet",
        DivinationCard = "Cartes divinatoires",
        StackableCurrency = "Objets monétaires empilables",
        Jewel = "Joyaux",
        DelveStackableSocketableCurrency = "Objets enchâssables et empilables Delve",
        MetamorphSample = "Organes de Métamorphe",
        HeistTool = "Outils de Malfaiteur",
        Amulet = "Amulettes",
        Ring = "Bagues",
        Belt = "Ceintures",
        Gloves = "Gants",
        Boots = "Bottes",
        BodyArmours = "Armures",
        Helmets = "Casques",
        Shields = "Boucliers",
        Quivers = "Carquois",
        LifeFlasks = "Flacons de Vie",
        ManaFlasks = "Flacons de Mana",
        HybridFlasks = "Flacons hybrides",
        UtilityFlasks = "Flacons utilitaires",
        ActiveSkillGems = "Gemmes d'aptitude",
        SupportSkillGems = "Gemmes de soutien",
        Maps = "Cartes",
        MapFragments = "Fragments de carte",
        Contract = "Contrats",
        Blueprint = "Plans",
        MiscMapItems = "Objets de Carte divers",
        Claws = "Griffes",
        Daggers = "Dagues",
        Wands = "Baguettes",
        OneHandSwords = "Épées à une main",
        ThrustingOneHandSwords = "Épées d'estoc à une main",
        OneHandAxes = "Haches à une main",
        OneHandMaces = "Masses à une main",
        Bows = "Arcs",
        Staves = "Bâtons",
        TwoHandSwords = "Épées à deux mains",
        TwoHandAxes = "Haches à deux mains",
        TwoHandMaces = "Masses à deux mains",
        Sceptres = "Sceptres",
        RuneDaggers = "Dagues runiques",
        Warstaves = "Bâtons de guerre",
        FishingRods = "Cannes à pêche",
        HeistGear = "Équipements de Malfaiteur",
        HeistBrooch = "Broches de Malfaiteur",
        HeistTarget = "Objectifs de Casse",
        HeistCloak = "Capes de Malfaiteur",
        AbyssJewel = "Joyaux abyssaux",
        Trinkets = "Babioles",
        Logbooks = "Journaux de bord d'Expédition",
        MemoryLine = "Souvenirs",
        SanctumRelics = "Reliques",
        Tinctures = "Teintures",
        Corpses = "Cadavres",
        SanctumResearch = "Recherche du Sanctuaire",
    };
}

