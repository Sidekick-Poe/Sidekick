using Sidekick.Common.Enums;

namespace Sidekick.Data;

public enum DataType
{
    [EnumValue("leagues.json")]
    Leagues,

    [EnumValue("{0}/items.json")]
    Items,

    [EnumValue("{0}/item-classes.json")]
    ItemClasses,

    [EnumValue("invariant-stats.json")]
    StatsInvariant,

    [EnumValue("{0}/stats.json")]
    Stats,

    [EnumValue("{0}/pseudo.json")]
    Pseudo,

    [EnumValue("{0}/trade-filters.json")]
    TradeFilters,

    [EnumValue("{0}/trade-stats.json")]
    TradeStats,
}