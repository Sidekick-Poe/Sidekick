namespace Sidekick.Common.Game.Languages.Implementations
{
    [GameLanguage("German", "de")]
    public class GameLanguageDE : IGameLanguage
    {
        public string LanguageCode => "de";

        public Uri PoeTradeSearchBaseUrl => new("https://de.pathofexile.com/trade/search/");
        public Uri PoeTradeExchangeBaseUrl => new("https://de.pathofexile.com/trade/exchange/");
        public Uri PoeTradeApiBaseUrl => new("https://de.pathofexile.com/api/trade/");
        public Uri PoeCdnBaseUrl => new("https://web.poecdn.com/");

        public string RarityUnique => "Einzigartig";
        public string RarityRare => "Selten";
        public string RarityMagic => "Magisch";
        public string RarityNormal => "Normal";
        public string RarityCurrency => "Währung";
        public string RarityGem => "Gemme";
        public string RarityDivinationCard => "Weissagungskarte";

        public string DescriptionUnidentified => "Nicht identifiziert";
        public string DescriptionQuality => "Qualität";
        public string DescriptionAlternateQuality => "Alternative Qualität";
        public string DescriptionIsRelic => "Einzigartiges Relikt";
        public string DescriptionCorrupted => "Verderbt";
        public string DescriptionScourged => "Transformiert";
        public string DescriptionSockets => "Fassungen";
        public string DescriptionItemLevel => "Gegenstandsstufe";
        public string DescriptionExperience => "Erfahrung";
        public string DescriptionMapTier => "Kartenlevel";
        public string DescriptionItemQuantity => "Gegenstandsmenge";
        public string DescriptionItemRarity => "Gegenstandsseltenheit";
        public string DescriptionMonsterPackSize => "Monstergruppengröße";
        public string DescriptionPhysicalDamage => "Physischer Schaden";
        public string DescriptionElementalDamage => "Elementarschaden";
        public string DescriptionAttacksPerSecond => "Angriffe pro Sekunde";
        public string DescriptionCriticalStrikeChance => "Kritische Trefferchance";
        public string DescriptionEnergyShield => "Energieschild";
        public string DescriptionArmour => "Rüstung";
        public string DescriptionEvasion => "Ausweichwert";
        public string DescriptionChanceToBlock => "__TranslationRequired__";
        public string DescriptionLevel => "__TranslationRequired__";
        public string DescriptionRequirements => "__TranslationRequired__:";
        public string DescriptionAreaLevel => "__TranslationRequired__:";

        public string AffixSuperior => "(hochwertig)";
        public string AffixBlighted => "Befallene";
        public string AffixBlightRavaged => "Extrem befallene";
        public string AffixAnomalous => "(anormal)";
        public string AffixDivergent => "(abweichend)";
        public string AffixPhantasmal => "(illusorisch)";

        public string InfluenceShaper => "Schöpfer";
        public string InfluenceElder => "Ältesten";
        public string InfluenceCrusader => "Kreuzritter";
        public string InfluenceHunter => "Jägers";
        public string InfluenceRedeemer => "Erlöserin";
        public string InfluenceWarlord => "Kriegsherrn";

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
