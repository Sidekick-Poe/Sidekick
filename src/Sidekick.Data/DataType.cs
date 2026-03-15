using Sidekick.Common.Enums;

namespace Sidekick.Data;

public enum DataType
{
    [EnumValue("ninja/exchange.json")]
    NinjaExchange,

    [EnumValue("ninja/stash.json")]
    NinjaStash,

    [EnumValue("items/{0}.json")]
    Items,

    [EnumValue("stats/{0}.json")]
    Stats,

    [EnumValue("pseudo/{0}.json")]
    Pseudo,

    [EnumValue("leagues.json")]
    Leagues,

    [EnumValue("stats/invariant.json")]
    StatsInvariant,

    [EnumValue("trade/filters.{0}.json")]
    TradeFilters,

    [EnumValue("raw/trade/filters.{0}.json")]
    RawTradeFilters,

    [EnumValue("raw/trade/items.{0}.json")]
    RawTradeItems,

    [EnumValue("raw/trade/leagues.{0}.json")]
    RawTradeLeagues,

    [EnumValue("raw/trade/static.{0}.json")]
    RawTradeStatic,

    [EnumValue("raw/trade/stats.{0}.json")]
    RawTradeStats,
}