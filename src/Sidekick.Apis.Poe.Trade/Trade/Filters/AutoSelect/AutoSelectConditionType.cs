namespace Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;

public enum AutoSelectConditionType
{
    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.IsContainedIn,
                          AutoSelectComparisonType.IsNotContainedIn)]
    ItemClass,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    ItemLevel,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    Quality,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.IsContainedIn,
                          AutoSelectComparisonType.IsNotContainedIn)]
    Rarity,

    [AutoSelectComparison(AutoSelectComparisonType.True,
                          AutoSelectComparisonType.False)]
    Corrupted,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    Spirit,

    [AutoSelectComparison(AutoSelectComparisonType.True,
                          AutoSelectComparisonType.False)]
    Foulborn,

    #region Equipment

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    SocketCount,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    Armour,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    EvasionRating,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    EnergyShield,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    BlockChance,

    #endregion

    #region Maps

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    AreaLevel,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    MapTier,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    ItemQuantity,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    ItemRarity,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    MagicMonsters,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    MonsterPackSize,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    RareMonsters,

    #endregion

    #region Weapons
    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    AttacksPerSecond,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    CriticalHitChance,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    PhysicalDps,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    TotalDps,
    #endregion

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    GemLevel,

    [AutoSelectComparison(AutoSelectComparisonType.MatchesRegex,
                          AutoSelectComparisonType.DoesNotMatchRegex)]
    AnyStat,

    [AutoSelectComparison(AutoSelectComparisonType.MatchesRegex,
                          AutoSelectComparisonType.DoesNotMatchRegex)]
    Text,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.IsContainedIn,
                          AutoSelectComparisonType.IsNotContainedIn)]
    StatCategory,

    [AutoSelectComparison(AutoSelectComparisonType.Equals,
                          AutoSelectComparisonType.DoesNotEqual,
                          AutoSelectComparisonType.GreaterThanOrEqual,
                          AutoSelectComparisonType.LesserThanOrEqual,
                          AutoSelectComparisonType.GreaterThan,
                          AutoSelectComparisonType.LesserThan)]
    Value,
}
