namespace Sidekick.Common.Game.Languages.Implementations
{
    [GameLanguage("Thai", "th")]
    public class GameLanguageTH : IGameLanguage
    {
        public string LanguageCode => "th";

        public Uri PoeTradeSearchBaseUrl => new("https://th.pathofexile.com/trade/search/");
        public Uri PoeTradeExchangeBaseUrl => new("https://th.pathofexile.com/trade/exchange/");
        public Uri PoeTradeApiBaseUrl => new("https://th.pathofexile.com/api/trade/");
        public Uri PoeCdnBaseUrl => new("https://web.poecdn.com/");

        public string RarityUnique => "Unique";
        public string RarityRare => "แรร์";
        public string RarityMagic => "เมจิก";
        public string RarityNormal => "ปกติ";
        public string RarityCurrency => "เคอเรนซี่";
        public string RarityGem => "เจ็ม";
        public string RarityDivinationCard => "ไพ่พยากรณ์";

        public string DescriptionUnidentified => "ยังไม่ได้ตรวจสอบ";
        public string DescriptionQuality => "คุณภาพ";
        public string DescriptionAlternateQuality => "คุณภาพแบบพิเศษ";
        public string DescriptionIsRelic => "ไอเทมยูนิคโบราณ";
        public string DescriptionCorrupted => "คอร์รัปต์";
        public string DescriptionScourged => "อาบมิติอสูร";
        public string DescriptionSockets => "ซ็อกเก็ต";
        public string DescriptionItemLevel => "เลเวลไอเทม";
        public string DescriptionMapTier => "ระดับแผนที่";
        public string DescriptionItemQuantity => "จำนวนไอเท็ม";
        public string DescriptionItemRarity => "ระดับความหายากของไอเทม";
        public string DescriptionMonsterPackSize => "ขนาดบรรจุมอนสเตอร์";
        public string DescriptionExperience => "ประสบการณ์";
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
        public string AffixBlighted => "Blighted";
        public string AffixBlightRavaged => "Blight-ravaged";
        public string AffixAnomalous => "Anomalous";
        public string AffixDivergent => "Divergent";
        public string AffixPhantasmal => "Phantasmal";

        public string InfluenceShaper => "เชปเปอร์";
        public string InfluenceElder => "เอลเดอร์";
        public string InfluenceCrusader => "ครูเซเดอร์";
        public string InfluenceHunter => "ฮันเตอร์";
        public string InfluenceRedeemer => "รีดีมเมอร์";
        public string InfluenceWarlord => "วอร์หลอด";

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
