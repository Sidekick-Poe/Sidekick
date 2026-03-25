namespace Sidekick.Apis.Poe.Account.Stash.Models;

public enum StashType
{
    Unknown,

    [StashTypeImage("/images/stashes/CurrencyStash.png")]
    Currency,

    [StashTypeImage("/images/stashes/EssenceStash.png")]
    Essences,

    [StashTypeImage("/images/stashes/UltimatumStash.png")]
    Ultimatum,

    Folder,

    Metamorph,

    [StashTypeImage("/images/stashes/DelveStash.png")]
    Delve,

    [StashTypeImage("/images/stashes/MapStash.png")]
    Map,

    [StashTypeImage("/images/stashes/BlightStash.png")]
    Blight,

    [StashTypeImage("/images/stashes/FragmentStash.png")]
    Fragment,

    [StashTypeImage("/images/stashes/DeliriumStash.png")]
    Delirium,

    [StashTypeImage("/images/stashes/DivinationCardStash.png")]
    DivinationCard,

    [StashTypeImage("/images/stashes/FlaskStash.png")]
    Flask,

    [StashTypeImage("/images/stashes/GemStash.png")]
    Gem,

    [StashTypeImage("/images/stashes/UniqueStash.png")]
    Unique,

    [StashTypeImage("/images/stashes/PremiumStash.png")]
    Premium,

    [StashTypeImage("/images/stashes/QuadStash.png")]
    Quad,
}
