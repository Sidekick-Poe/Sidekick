using Sidekick.Common.Enums;
namespace Sidekick.Apis.Poe.Items;

public enum GameType
{
    Unknown = 0,

    [EnumValue("poe1")]
    PathOfExile = 1,

    [EnumValue("poe2")]
    PathOfExile2 = 2,
}
