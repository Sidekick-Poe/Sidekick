using Sidekick.Common.Enums;
namespace Sidekick.Apis.Poe.Account.Stash.Models;

public enum StashType
{
    Unknown,

    [EnumValue(key: "image", value: "/images/stashes/CurrencyStash.png")]
    Currency,

    [EnumValue(key: "image", value: "/images/stashes/EssenceStash.png")]
    Essences,

    [EnumValue(key: "image", value: "/images/stashes/UltimatumStash.png")]
    Ultimatum,

    Folder,
    Metamorph,

    [EnumValue(key: "image", value: "/images/stashes/DelveStash.png")]
    Delve,

    [EnumValue(key: "image", value: "/images/stashes/MapStash.png")]
    Map,

    [EnumValue(key: "image", value: "/images/stashes/BlightStash.png")]
    Blight,

    [EnumValue(key: "image", value: "/images/stashes/FragmentStash.png")]
    Fragment,

    [EnumValue(key: "image", value: "/images/stashes/DeliriumStash.png")]
    Delirium,

    [EnumValue(key: "image", value: "/images/stashes/DivinationCardStash.png")]
    DivinationCard,

    [EnumValue(key: "image", value: "/images/stashes/FlaskStash.png")]
    Flask,

    [EnumValue(key: "image", value: "/images/stashes/GemStash.png")]
    Gem,

    [EnumValue(key: "image", value: "/images/stashes/UniqueStash.png")]
    Unique,

    [EnumValue(key: "image", value: "/images/stashes/PremiumStash.png")]
    Premium,

    [EnumValue(key: "image", value: "/images/stashes/QuadStash.png")]
    Quad,
}
