using Sidekick.Common.Enums;

namespace Sidekick.Common.Settings;

public enum RangeType
{
    [EnumValue("minimum")]
    Minimum,
    [EnumValue("maximum")]
    Maximum,
    [EnumValue("equals")]
    Equals,
    [EnumValue("range")]
    Range,
}