namespace Sidekick.Common.Game.Languages.Implementations
{
    [GameLanguage("Spanish", "es")]
    public class GameLanguageES : IGameLanguage
    {
        public string LanguageCode => "es";

        public Uri PoeTradeSearchBaseUrl => new("https://es.pathofexile.com/trade/search/");
        public Uri PoeTradeExchangeBaseUrl => new("https://es.pathofexile.com/trade/exchange/");
        public Uri PoeTradeApiBaseUrl => new("https://es.pathofexile.com/api/trade/");
        public Uri PoeCdnBaseUrl => new("https://web.poecdn.com/");

        public string RarityUnique => "Único";
        public string RarityRare => "Raro";
        public string RarityMagic => "Mágico";
        public string RarityNormal => "Normal";
        public string RarityCurrency => "Objetos Monetarios";
        public string RarityGem => "Gema";
        public string RarityDivinationCard => "Carta de Adivinación";

        public string DescriptionUnidentified => "Sin identificar";
        public string DescriptionQuality => "Calidad";
        public string DescriptionAlternateQuality => "Calidad alternativa";
        public string DescriptionIsRelic => "Reliquia única";
        public string DescriptionCorrupted => "Corrupto";
        public string DescriptionScourged => "de calamidad";
        public string DescriptionSockets => "Engarces";
        public string DescriptionItemLevel => "Nivel de objeto";
        public string DescriptionExperience => "Experiencia";
        public string DescriptionMapTier => "Grado del mapa";
        public string DescriptionItemQuantity => "Cantidad de objetos";
        public string DescriptionItemRarity => "Rareza de objetos";
        public string DescriptionMonsterPackSize => "Tamaño de los grupos de monstruos";
        public string DescriptionPhysicalDamage => "__TranslationRequired__";
        public string DescriptionElementalDamage => "__TranslationRequired__";
        public string DescriptionAttacksPerSecond => "__TranslationRequired__";
        public string DescriptionCriticalStrikeChance => "__TranslationRequired__";
        public string DescriptionEnergyShield => "__TranslationRequired__";
        public string DescriptionArmour => "__TranslationRequired__";
        public string DescriptionEvasion => "__TranslationRequired__";
        public string DescriptionChanceToBlock => "__TranslationRequired__";
        public string DescriptionLevel => "__TranslationRequired__";
        public string DescriptionRequirements => "__TranslationRequired__:";
        public string DescriptionAreaLevel => "__TranslationRequired__:";

        public string AffixSuperior => "Superior";
        public string AffixBlighted => "Infestado";
        public string AffixBlightRavaged => "devastado";
        public string AffixAnomalous => "anómala";
        public string AffixDivergent => "divergente";
        public string AffixPhantasmal => "fantasmal";

        public string InfluenceShaper => "Creador";
        public string InfluenceElder => "Antiguo";
        public string InfluenceCrusader => "Cruzado";
        public string InfluenceHunter => "Cazador";
        public string InfluenceRedeemer => "Redentora";
        public string InfluenceWarlord => "Jefe de guerra";

        public ClassLanguage? Classes => new()
        {
            Prefix = "___",
            DivinationCard = "___",
            StackableCurrency = "___",
            Jewel = "___",
            DelveStackableSocketableCurrency = "___",
            MetamorphSample = "___",
            HeistTool = "___",
            Amulet = "___",
            Ring = "___",
            Belt = "___",
            Gloves = "___",
            Boots = "___",
            BodyArmours = "___",
            Helmets = "___",
            Shields = "___",
            Quivers = "___",
            LifeFlasks = "___",
            ManaFlasks = "___",
            HybridFlasks = "___",
            UtilityFlasks = "___",
            ActiveSkillGems = "___",
            SupportSkillGems = "___",
            Maps = "___",
            MapFragments = "___",
            Contract = "___",
            Blueprint = "___",
            MiscMapItems = "___",
            Claws = "___",
            Daggers = "___",
            Wands = "___",
            OneHandSwords = "___",
            ThrustingOneHandSwords = "___",
            OneHandAxes = "___",
            OneHandMaces = "___",
            Bows = "___",
            Staves = "___",
            TwoHandSwords = "___",
            TwoHandAxes = "___",
            TwoHandMaces = "___",
            Sceptres = "___",
            RuneDaggers = "___",
            Warstaves = "___",
            FishingRods = "___",
            HeistGear = "___",
            HeistBrooch = "___",
            HeistTarget = "___",
            HeistCloak = "___",
            AbyssJewel = "___",
            Trinkets = "___",
            Logbooks = "___",
            MemoryLine = "___",
            SanctumResearch = "___",
        };
    }
}
