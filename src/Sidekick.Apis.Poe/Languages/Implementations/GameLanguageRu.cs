
namespace Sidekick.Apis.Poe.Languages.Implementations;

[GameLanguage("Russian", "ru")]
public class GameLanguageRu : IGameLanguage
{
    public string Code => "ru";

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

    public string DescriptionRarity => "Редкость";
    public string DescriptionUnidentified => "Неопознано";
    public string DescriptionQuality => "Качество";
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
    public string DescriptionEnergyShieldAlternate => "Энергетический щит";
    public string DescriptionArmour => "Броня";
    public string DescriptionEvasion => "Уклонение";
    public string DescriptionChanceToBlock => "Шанс заблокировать удар";
    public string DescriptionBlockChance => "Шанс блока";
    public string DescriptionSpirit => "Дух";
    public string DescriptionAttacksPerSecond => "Атак в секунду";
    public string DescriptionCriticalStrikeChance => "Шанс критического удара";
    public string DescriptionCriticalHitChance => "Шанс крит. попадания";
    public string DescriptionMapTier => "Уровень карты";
    public string DescriptionReward => "Награда";
    public string DescriptionItemQuantity => "Количество предметов";
    public string DescriptionItemRarity => "Редкость предметов";
    public string DescriptionMonsterPackSize => "Размер групп монстров";
    public string DescriptionMagicMonsters => "Волшебные монстры";
    public string DescriptionRareMonsters => "Редкие монстры";
    public string DescriptionRevivesAvailable => "Доступно возрождений";
    public string DescriptionWaystoneDropChance => "Шанс выпадения путевого камня";
    public string DescriptionAreaLevel => "Уровень области";
    public string DescriptionUnusable => "Вы не можете использовать этот предмет, его параметры не будут учтены";
    public string DescriptionRequirements => "Требования";
    public string DescriptionRequires => "Требуется";
    public string DescriptionRequiresLevel => "Уровень";
    public string DescriptionRequiresStr => "Сила";
    public string DescriptionRequiresDex => "Ловк";
    public string DescriptionRequiresInt => "Инт";

    public string AffixSuperior => "высокого качества";
    public string AffixBlighted => "Заражённая";
    public string AffixBlightRavaged => "Разорённая Скверной";

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
        Socketable = "Размещаемое",
        Omen = "Предзнаменования",
        Jewel = "Самоцветы",
        DelveStackableSocketableCurrency = "Валюта Спуска",
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
        Focus = "Фокусы",
        LifeFlasks = "Флаконы жизни",
        ManaFlasks = "Флаконы маны",
        HybridFlasks = "Флаконы равновесия",
        UtilityFlasks = "Особые флаконы",
        ActiveSkillGems = "Камни умений",
        SupportSkillGems = "Камни поддержки",
        UncutSkillGems = "Неогранённые камни умений",
        UncutSupportGems = "Неогранённые камни поддержки",
        UncutSpiritGems = "Неогранённые камни духа",
        Maps = "Карты",
        MapFragments = "Обрывки карт",
        Contract = "Контракты",
        Blueprint = "Чертежи",
        MiscMapItems = "Прочие предметы карт",
        Waystone = "Путевые камни",
        Barya = "Монеты Испытания",
        Ultimatum = "Начертанные Ультиматумы",
        Tablet = "Плитки",
        Breachstone = "Камни Разлома",
        BossKey = "Древние ключи",
        Claws = "Когти",
        Daggers = "Кинжалы",
        Wands = "Жезлы",
        OneHandSwords = "Одноручные мечи",
        ThrustingOneHandSwords = "Шпаги",
        OneHandAxes = "Одноручные топоры",
        OneHandMaces = "Одноручные булавы",
        Bows = "Луки",
        Crossbows = "Самострелы",
        Staves = "Посохи",
        TwoHandSwords = "Двуручные мечи",
        TwoHandAxes = "Двуручные топоры",
        TwoHandMaces = "Двуручные булавы",
        Sceptres = "Скипетры",
        RuneDaggers = "Рунические кинжалы",
        Warstaves = "Воинские посохи",
        Quarterstaves = "Боевые посохи",
        Spears = "Копья",
        Bucklers = "Баклеры",
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

