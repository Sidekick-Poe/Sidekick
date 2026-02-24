namespace Sidekick.Data.Trade.Models;

public class TradeInvariantStats
{
    public List<string> IgnoreStatIds { get; init; } = [];

    public List<string> IncursionRoomStatIds { get; init; } = [];

    public List<string> LogbookFactionStatIds { get; init; } = [];

    public List<string> FireWeaponDamageIds { get; init; } = [];

    public List<string> ColdWeaponDamageIds { get; init; } = [];

    public List<string> LightningWeaponDamageIds { get; init; } = [];

    public string ClusterJewelSmallPassiveCountStatId { get; init; } = string.Empty;

    public string ClusterJewelSmallPassiveGrantStatId { get; init; } = string.Empty;

    public Dictionary<int, string> ClusterJewelSmallPassiveGrantOptions { get; init; } = [];
}
