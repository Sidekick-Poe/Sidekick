using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters;

public enum StatType
{
    [EnumValue("and")]
    And,

    [EnumValue("count")]
    Count,

    [EnumValue("weight2")]
    WeightedSum,
}
