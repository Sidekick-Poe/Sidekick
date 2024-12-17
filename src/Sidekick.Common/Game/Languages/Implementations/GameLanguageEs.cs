namespace Sidekick.Common.Game.Languages.Implementations;

[GameLanguage("Spanish", "es")]
public class GameLanguageES : IGameLanguage
{
    public string PoeTradeBaseUrl => "https://es.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://es.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://es.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://es.pathofexile.com/api/trade2/";
    public Uri PoeCdnBaseUrl => new("https://web.poecdn.com/");

    public string RarityUnique => "Único";
    public string RarityRare => "Raro";
    public string RarityMagic => "Mágico";
    public string RarityNormal => "Normal";
    public string RarityCurrency => "Objetos monetarios";
    public string RarityGem => "Gema";
    public string RarityDivinationCard => "Carta de adivinación";

    public string DescriptionUnidentified => "Sin identificar";
    public string DescriptionQuality => "Calidad";
    public string DescriptionAlternateQuality => "Calidad alternativa";
    public string DescriptionLevel => "16";
    public string DescriptionCorrupted => "Corrupto";
    public string DescriptionSockets => "Engarces";
    public string DescriptionItemLevel => "Nivel de objeto";
    public string DescriptionExperience => "Experiencia";
    public string DescriptionPhysicalDamage => "Daño físico";
    public string DescriptionElementalDamage => "Daño elemental";
    public string DescriptionFireDamage => "Daño de fuego";
    public string DescriptionColdDamage => "Daño de hielo";
    public string DescriptionLightningDamage => "Daño de rayo";
    public string DescriptionChaosDamage => "Daño de caos";
    public string DescriptionEnergyShield => "Escudo de energía";
    public string DescriptionArmour => "Armadura";
    public string DescriptionEvasion => "Evasión";
    public string DescriptionChanceToBlock => "Probabilidad de bloqueo";
    public string DescriptionAttacksPerSecond => "Ataques por segundo";
    public string DescriptionCriticalStrikeChance => "Daño de golpe crítico";
    public string DescriptionMapTier => "Grado del mapa";
    public string DescriptionItemQuantity => "Cantidad de objetos";
    public string DescriptionItemRarity => "Rareza de objetos";
    public string DescriptionMonsterPackSize => "Tamaño de los grupos de monstruos";
    public string DescriptionRequirements => "Requisitos";
    public string DescriptionAreaLevel => "Nivel del área";

    public string AffixSuperior => "Superior";
    public string AffixBlighted => "infestado";
    public string AffixBlightRavaged => "devastado por la plaga";
    public string AffixAnomalous => "anómala";
    public string AffixDivergent => "divergente";
    public string AffixPhantasmal => "fantasmal";

    public string InfluenceShaper => "Objeto del Creador";
    public string InfluenceElder => "Objeto del Antiguo";
    public string InfluenceCrusader => "Objeto del Cruzado";
    public string InfluenceHunter => "Objeto del Cazador";
    public string InfluenceRedeemer => "Objeto de la Redentora";
    public string InfluenceWarlord => "Objeto del Jefe de guerra";

    public ClassLanguage Classes { get; } = new()
    {
        Prefix = "Clase de objeto",
        DivinationCard = "Cartas de adivinación",
        StackableCurrency = "Objetos monetarios apilables",
        Jewel = "Joyas",
        DelveStackableSocketableCurrency = "Objetos monetarios apilables y engarzables de Delve",
        MetamorphSample = "Muestras de metamorfo",
        HeistTool = "Herramientas de Heist",
        Amulet = "Amuletos",
        Ring = "Anillos",
        Belt = "Cinturones",
        Gloves = "Guantes",
        Boots = "Botas",
        BodyArmours = "Armaduras corporales",
        Helmets = "Cascos",
        Shields = "Escudos",
        Quivers = "Carcajes",
        LifeFlasks = "Frascos de vida",
        ManaFlasks = "Frascos de maná",
        HybridFlasks = "Frascos híbridos",
        UtilityFlasks = "Frascos de utilidad",
        ActiveSkillGems = "Gemas de habilidad",
        SupportSkillGems = "Gemas de asistencia",
        Maps = "Mapas",
        MapFragments = "Fragmentos de mapa",
        Contract = "Contratos",
        Blueprint = "Planos",
        MiscMapItems = "Objetos misceláneos de mapa",
        Claws = "Garras",
        Daggers = "Dagas",
        Wands = "Varitas",
        OneHandSwords = "Espadas a una mano",
        ThrustingOneHandSwords = "Espadas agresivas a una mano",
        OneHandAxes = "Hachas a una mano",
        OneHandMaces = "Mazas a una mano",
        Bows = "Arcos",
        Staves = "Báculos",
        TwoHandSwords = "Espadas a dos manos",
        TwoHandAxes = "Hachas a dos manos",
        TwoHandMaces = "Mazas a dos manos",
        Sceptres = "Cetros",
        RuneDaggers = "Dagas rúnicas",
        Warstaves = "Báculos de guerra",
        FishingRods = "Cañas de pescar",
        HeistGear = "Accesorio de Heist",
        HeistBrooch = "Broches de Heist",
        HeistTarget = "Objetivos de Heist",
        HeistCloak = "Capas de Heist",
        AbyssJewel = "Joyas de abismo",
        Trinkets = "Abalorios",
        Logbooks = "Registros de expedición",
        MemoryLine = "Recuerdos",
        SanctumRelics = "Reliquias",
        Tinctures = "Tinturas",
        Corpses = "Cadáveres",
        SanctumResearch = "Investigación del santuario",
    };
}

