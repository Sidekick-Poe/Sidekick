namespace Sidekick.Common.Game.Languages.Implementations
{
    [GameLanguage("Portuguese", "pt")]
    public class GameLanguagePT : IGameLanguage
    {
        public string LanguageCode => "pt";

        public Uri PoeTradeSearchBaseUrl => new("https://br.pathofexile.com/trade/search/");
        public Uri PoeTradeExchangeBaseUrl => new("https://br.pathofexile.com/trade/exchange/");
        public Uri PoeTradeApiBaseUrl => new("https://br.pathofexile.com/api/trade/");
        public Uri PoeCdnBaseUrl => new("https://web.poecdn.com/");

        public string RarityUnique => "Único";
        public string RarityRare => "Raro";
        public string RarityMagic => "Mágico";
        public string RarityNormal => "Normal";
        public string RarityCurrency => "Moeda";
        public string RarityGem => "Gema";
        public string RarityDivinationCard => "Carta de Adivinhação";

        public string DescriptionUnidentified => "Não Identificado";
        public string DescriptionQuality => "Qualidade";
        public string DescriptionAlternateQuality => "Qualidade Alternativa";
        public string DescriptionIsRelic => "Único Relíquia";
        public string DescriptionCorrupted => "Corrompido";
        public string DescriptionScourged => "Castigado";
        public string DescriptionSockets => "Encaixes";
        public string DescriptionItemLevel => "Nível do Item";
        public string DescriptionExperience => "Experiência";
        public string DescriptionMapTier => "Tier do Mapa";
        public string DescriptionItemQuantity => "Quantidade de Itens";
        public string DescriptionItemRarity => "Raridade de Itens";
        public string DescriptionMonsterPackSize => "Tamanho do Grupo de Monstros";
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
        public string AffixBlighted => "Arruinado";
        public string AffixBlightRavaged => "Devastado";
        public string AffixAnomalous => "Anômalo";
        public string AffixDivergent => "Divergente";
        public string AffixPhantasmal => "Fantasmal";

        public string InfluenceShaper => "Criador";
        public string InfluenceElder => "Ancião";
        public string InfluenceCrusader => "Cruzado";
        public string InfluenceHunter => "Caçador";
        public string InfluenceRedeemer => "Redentor";
        public string InfluenceWarlord => "Senhor da Guerra";

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
