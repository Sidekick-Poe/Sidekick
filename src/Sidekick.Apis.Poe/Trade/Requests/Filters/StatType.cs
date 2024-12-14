using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters
{
    internal enum StatType
    {
        [EnumValue("and")]
        And,

        [EnumValue("count")]
        Count,

        [EnumValue("not")]
        Not,

        [EnumValue("if")]
        If
    }
}
