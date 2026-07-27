using Sidekick.Common.Enums;

namespace Sidekick.Data;

public enum DataType
{
    [EnumValue("items/{0}.json")]
    Items,

    [EnumValue("item-classes/{0}.json")]
    ItemClasses,

    [EnumValue("stats/{0}.json")]
    Stats,

    [EnumValue("stats/trade.{0}.json")]
    TradeStats,

    [EnumValue("pseudo/{0}.json")]
    Pseudo,

    [EnumValue("leagues.json")]
    Leagues,

    [EnumValue("stats/invariant.json")]
    StatsInvariant,

    [EnumValue("trade/filters.{0}.json")]
    TradeFilters,
}