using Sidekick.Common.Enums;

namespace Sidekick.Game;

public enum GameDataType
{
    [EnumValue("leagues.json")]
    Leagues,

    [EnumValue("{0}/base-items.json")]
    BaseItems,

    [EnumValue("{0}/items.json")]
    Items,

    [EnumValue("{0}/item-classes.json")]
    ItemClasses,

    [EnumValue("ninja-exchange-items.json")]
    NinjaExchangeItems,

    [EnumValue("ninja-stash-items.json")]
    NinjaStashItems,

    [EnumValue("scout-items.json")]
    ScoutItems,

    [EnumValue("invariant-stats.json")]
    StatsInvariant,

    [EnumValue("{0}/stats.json")]
    Stats,

    [EnumValue("{0}/pseudo.json")]
    Pseudo,

    [EnumValue("{0}/texts.json")]
    Texts,

    [EnumValue("{0}/trade-filters.json")]
    TradeFilters,

    [EnumValue("{0}/trade-stats.json")]
    TradeStats,
}