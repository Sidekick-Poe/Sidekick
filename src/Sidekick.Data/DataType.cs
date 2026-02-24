using Sidekick.Common.Enums;

namespace Sidekick.Data;

public enum DataType
{
    [EnumValue("trade")]
    Trade,

    [EnumValue("ninja")]
    PoeNinja,

    [EnumValue("game")]
    Game,
}