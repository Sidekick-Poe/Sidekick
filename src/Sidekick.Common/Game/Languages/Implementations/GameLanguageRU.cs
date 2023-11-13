namespace Sidekick.Common.Game.Languages.Implementations
{
    [GameLanguage("Russian", "ru")]
    public class GameLanguageRU : IGameLanguage
    {
        public string LanguageCode => "ru";

        public Uri PoeTradeSearchBaseUrl => new("https://ru.pathofexile.com/trade/search/");
        public Uri PoeTradeExchangeBaseUrl => new("https://ru.pathofexile.com/trade/exchange/");
        public Uri PoeTradeApiBaseUrl => new("https://ru.pathofexile.com/api/trade/");
        public Uri PoeCdnBaseUrl => new("https://web.poecdn.com/");

        public string RarityUnique => "Уникальный";
        public string RarityRare => "Редкий";
        public string RarityMagic => "Волшебный";
        public string RarityNormal => "Обычный";
        public string RarityCurrency => "Валюта";
        public string RarityGem => "Камень";
        public string RarityDivinationCard => "Гадальная карта";

        public string DescriptionUnidentified => "Неопознано";
        public string DescriptionQuality => "Качество";
        public string DescriptionAlternateQuality => "Изменённый эффект качества";
        public string DescriptionIsRelic => "Уникальная Реликвия";
        public string DescriptionCorrupted => "Осквернено";
        public string DescriptionScourged => "Преображено";
        public string DescriptionSockets => "Гнезда";
        public string DescriptionItemLevel => "Уровень предмета";
        public string DescriptionMapTier => "Уровень карты";
        public string DescriptionItemQuantity => "Количество предметов";
        public string DescriptionItemRarity => "Редкость предметов";
        public string DescriptionMonsterPackSize => "Размер групп монстров";
        public string DescriptionExperience => "Опыт";
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

        public string AffixSuperior => "Рог";
        public string AffixBlighted => "Заражённая";
        public string AffixBlightRavaged => "Разорённая Скверной";
        public string AffixAnomalous => "Аномальный: ";
        public string AffixDivergent => "Искривлённый: ";
        public string AffixPhantasmal => "Фантомный: ";

        public string InfluenceShaper => "Создателя";
        public string InfluenceElder => "Древнего";
        public string InfluenceCrusader => "Крестоносца";
        public string InfluenceHunter => "Охотника";
        public string InfluenceRedeemer => "Избавительницы";
        public string InfluenceWarlord => "Вождя";

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
