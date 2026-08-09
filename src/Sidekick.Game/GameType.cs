using Sidekick.Common.Enums;

namespace Sidekick.Game;

public enum GameType : byte
{
    Unknown = 0,

    [EnumValue("poe1")]
    PathOfExile1 = 1,

    [EnumValue("poe2")]
    PathOfExile2 = 2,
}
