using Sidekick.Common.Enums;

namespace Sidekick.Data;

public enum DataType
{
    [EnumValue("ninja/exchange.json")]
    NinjaExchange,

    [EnumValue("ninja/stash.json")]
    NinjaStash,

    [EnumValue("stats/{0}.json")]
    Stats,

    [EnumValue("pseudo/{0}.json")]
    Pseudo,

    [EnumValue("trade/leagues.invariant.json")]
    TradeLeagues,

    [EnumValue("trade/stats.{0}.json")]
    TradeStats,

    [EnumValue("trade/stats.invariant.json")]
    TradeInvariantStats,

    [EnumValue("trade/raw/filters.{0}.json")]
    TradeRawFilters,

    [EnumValue("trade/raw/items.{0}.json")]
    TradeRawItems,

    [EnumValue("trade/raw/leagues.{0}.json")]
    TradeRawLeagues,

    [EnumValue("trade/raw/static.{0}.json")]
    TradeRawStatic,

    [EnumValue("trade/raw/stats.{0}.json")]
    TradeRawStats,
}