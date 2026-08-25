using Sidekick.Common.Enums;

namespace Sidekick.Game;

public enum GameType : byte
{
    Unknown = 0,

    [EnumValue("poe1")]
    Poe1 = 1,

    [EnumValue("poe2")]
    Poe2 = 2,
}
