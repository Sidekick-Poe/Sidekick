using Sidekick.Common.Enums;

namespace Sidekick.Common.Settings;

public enum FilterType
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