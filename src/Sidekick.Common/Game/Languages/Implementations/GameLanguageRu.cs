namespace Sidekick.Common.Game.Languages.Implementations;

[GameLanguage("Russian", "ru")]
public class GameLanguageRU : IGameLanguage
{
    public string PoeTradeBaseUrl => "https://ru.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://ru.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://ru.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://ru.pathofexile.com/api/trade2/";
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
    public string DescriptionLevel => "Уровень";
    public string DescriptionCorrupted => "Осквернено";
    public string DescriptionSockets => "Гнезда";
    public string DescriptionItemLevel => "Уровень предмета";
    public string DescriptionExperience => "Опыт";
    public string DescriptionPhysicalDamage => "Физический урон";
    public string DescriptionElementalDamage => "Урон от стихий";
    public string DescriptionFireDamage => "Урон от огня";
    public string DescriptionColdDamage => "Урон от холода";
    public string DescriptionLightningDamage => "Урон от молнии";
    public string DescriptionChaosDamage => "Урон хаосом";
    public string DescriptionEnergyShield => "Энерг. щит";
    public string DescriptionArmour => "Броня";
    public string DescriptionEvasion => "Уклонение";
    public string DescriptionChanceToBlock => "Шанс заблокировать удар";
    public string DescriptionAttacksPerSecond => "Атак в секунду";
    public string DescriptionCriticalStrikeChance => "Шанс критического удара";
    public string DescriptionMapTier => "Уровень карты";
    public string DescriptionItemQuantity => "Количество предметов";
    public string DescriptionItemRarity => "Редкость предметов";
    public string DescriptionMonsterPackSize => "Размер групп монстров";
    public string DescriptionRequirements => "Требования";
    public string DescriptionAreaLevel => "Уровень области";

    public string AffixSuperior => "высокого качества";
    public string AffixBlighted => "Заражённая";
    public string AffixBlightRavaged => "Разорённая Скверной";
    public string AffixAnomalous => "Аномальный:";
    public string AffixDivergent => "Искривлённый:";
    public string AffixPhantasmal => "Фантомный:";

    public string InfluenceShaper => "Предмет Создателя";
    public string InfluenceElder => "Древний предмет";
    public string InfluenceCrusader => "Предмет Крестоносца";
    public string InfluenceHunter => "Предмет Охотника";
    public string InfluenceRedeemer => "Предмет Избавительницы";
    public string InfluenceWarlord => "Предмет Вождя";

    public ClassLanguage Classes { get; } = new()
    {
        Prefix = "Класс предмета",
        DivinationCard = "Гадальные карты",
        StackableCurrency = "Валюта",
        Jewel = "Самоцветы",
        DelveStackableSocketableCurrency = "Валюта Спуска",
        MetamorphSample = "Образцы Метаморфа",
        HeistTool = "Разбойничий инструмент",
        Amulet = "Амулеты",
        Ring = "Кольца",
        Belt = "Пояса",
        Gloves = "Перчатки",
        Boots = "Обувь",
        BodyArmours = "Доспехи",
        Helmets = "Шлемы",
        Shields = "Щиты",
        Quivers = "Колчаны",
        LifeFlasks = "Флаконы жизни",
        ManaFlasks = "Флаконы маны",
        HybridFlasks = "Флаконы равновесия",
        UtilityFlasks = "Особые флаконы",
        ActiveSkillGems = "Камни умений",
        SupportSkillGems = "Камни поддержки",
        Maps = "Карты",
        MapFragments = "Обрывки карт",
        Contract = "Контракты",
        Blueprint = "Чертежи",
        MiscMapItems = "Прочие предметы карт",
        Claws = "Когти",
        Daggers = "Кинжалы",
        Wands = "Жезлы",
        OneHandSwords = "Одноручные мечи",
        ThrustingOneHandSwords = "Шпаги",
        OneHandAxes = "Одноручные топоры",
        OneHandMaces = "Одноручные булавы",
        Bows = "Луки",
        Staves = "Посохи",
        TwoHandSwords = "Двуручные мечи",
        TwoHandAxes = "Двуручные топоры",
        TwoHandMaces = "Двуручные булавы",
        Sceptres = "Скипетры",
        RuneDaggers = "Рунические кинжалы",
        Warstaves = "Воинские посохи",
        FishingRods = "Удочки",
        HeistGear = "Разбойничьи принадлежности",
        HeistBrooch = "Разбойничьи броши",
        HeistTarget = "Предметы кражи",
        HeistCloak = "Разбойничьи накидки",
        AbyssJewel = "Самоцветы Бездны",
        Trinkets = "Украшения",
        Logbooks = "Журналы экспедиции",
        MemoryLine = "Воспоминания",
        SanctumRelics = "Реликвии",
        Tinctures = "Микстуры",
        Corpses = "Трупы",
        SanctumResearch = "Исследования Святилища",
    };
}

