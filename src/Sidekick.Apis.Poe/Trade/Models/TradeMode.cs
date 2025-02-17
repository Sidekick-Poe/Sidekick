using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Models;

public enum TradeMode
{
    [EnumValue("item")]
    Item,

    [EnumValue("bulk")]
    Bulk,
}
