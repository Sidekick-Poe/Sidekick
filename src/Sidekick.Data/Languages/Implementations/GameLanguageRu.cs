
namespace Sidekick.Data.Languages.Implementations;

public class GameLanguageRu : IGameLanguage
{
    public string Code => "ru";
    public string Label => "Russian";

    public string PoeTradeBaseUrl => "https://ru.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://ru.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://ru.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://ru.pathofexile.com/api/trade2/";

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
    public string DescriptionSplit => "Разделено";
    public string DescriptionMirrored => "Отражено";
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
    public string DescriptionMoreMaps => "Больше карт";
    public string DescriptionMoreScarabs => "Больше скарабеев";
    public string DescriptionMoreCurrency => "Больше валюты";
    public string DescriptionMoreCards => "Больше гадальных карт";
    public string DescriptionQualityCurrency => "Качество (валюта)";
    public string DescriptionQualityScarabs => "Качество (скарабеи)";
    public string DescriptionQualityCards => "Качество (гадальные карты)";
    public string DescriptionQualityPackSize => "Качество (размер группы)";
    public string DescriptionQualityRarity => "Качество (редкость)";
    public string DescriptionMagicMonsters => "Волшебные монстры";
    public string DescriptionRareMonsters => "Редкие монстры";
    public string DescriptionRevivesAvailable => "Доступно возрождений";
    public string DescriptionWaystoneDropChance => "Шанс выпадения путевого камня";
    public string DescriptionAreaLevel => "Уровень области";
    public string DescriptionMemoryStrands => "Пряди воспоминаний";
    public string DescriptionUnusable => "Вы не можете использовать этот предмет, его параметры не будут учтены";
    public string DescriptionRequirements => "Требования";
    public string DescriptionRequires => "Требуется";
    public string DescriptionRequiresLevel => "Уровень";
    public string DescriptionRequiresStr => "Сила";
    public string DescriptionRequiresDex => "Ловк";
    public string DescriptionRequiresInt => "Инт";
    public string DescriptionHeistWings => "Крыльев обнаружено";
    public string DescriptionHeistRoutes => "Путей отхода обнаружено";
    public string DescriptionHeistRooms => "Комнат с наградами обнаружено";
    public string DescriptionHeistLockpicking => "Требуется взлом (# уровень)";
    public string DescriptionHeistDemolition => "Требуется взрывное дело (# уровень)";
    public string DescriptionHeistAgility => "Требуется проворство (# уровень)";
    public string DescriptionHeistCounterThaumaturgy => "Требуется контрмагия (# уровень)";
    public string DescriptionHeistTrap => "Требуется разминирование (# уровень)";
    public string DescriptionHeistPerception => "Требуется восприятие (# уровень)";
    public string DescriptionHeistBruteForce => "Требуется грубая сила (# уровень)";
    public string DescriptionHeistDeception => "Требуется маскировка (# уровень)";
    public string DescriptionHeistEngineering => "Требуется инженерное дело (# уровень)";
    public string DescriptionHeistModerateValue => "средней ценности";
    public string DescriptionHeistHighValue => "ценный";
    public string DescriptionHeistPrecious => "драгоценный";
    public string DescriptionHeistPriceless => "бесценный";

    public string AffixSuperior => "высокого качества";
    public string AffixBlighted => "Заражённая";
    public string AffixBlightRavaged => "Разорённая Скверной";

    public string InfluenceShaper => "Предмет Создателя";
    public string InfluenceElder => "Древний предмет";
    public string InfluenceCrusader => "Предмет Крестоносца";
    public string InfluenceHunter => "Предмет Охотника";
    public string InfluenceRedeemer => "Предмет Избавительницы";
    public string InfluenceWarlord => "Предмет Вождя";
}

