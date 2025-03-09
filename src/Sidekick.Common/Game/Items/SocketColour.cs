namespace Sidekick.Common.Game.Items;

public enum SocketColour
{
    Undefined,

    Blue,
    Green,
    Red,
    White,
    Abyss,

    // The following three socket colours are to support Path of Exile 2
    // This socket represents an empty socket.
    PoE2,

    // This socket represents a socket with a soulcore socketed in it.
    PoE2_Soulcore,

    // This socket represents a socket with a rune socketed in it.
    PoE2_Rune,

    // This socket represents a gem socket.
    PoE2_Gem,
}
