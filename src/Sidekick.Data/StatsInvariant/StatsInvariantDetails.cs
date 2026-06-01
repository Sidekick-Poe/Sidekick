namespace Sidekick.Data.StatsInvariant;

public class StatsInvariantDetails
{
    public List<string> IgnoreStatIds { get; init; } = [];

    public List<string> IncursionRoomStatIds { get; init; } = [];

    public List<string> LogbookFactionStatIds { get; init; } = [];

    public List<string> LogbookBossStatIds { get; init; } = [];

    public List<string> FireWeaponDamageIds { get; init; } = [];

    public List<string> ColdWeaponDamageIds { get; init; } = [];

    public List<string> LightningWeaponDamageIds { get; init; } = [];
}
