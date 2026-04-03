using Sidekick.Common.Enums;
namespace Sidekick.Data.Items;

public enum HeistObjectiveValue
{
    Undefined,

    [EnumValue("moderate")]
    Moderate,

    [EnumValue("high")]
    High,

    [EnumValue("precious")]
    Precious,

    [EnumValue("priceless")]
    Priceless,
}
