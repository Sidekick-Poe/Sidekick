using Sidekick.Common.Initialization;
using Sidekick.Data.Trade.Models;
namespace Sidekick.Apis.Poe.Trade.ApiStats;

public interface IInvariantStatsProvider : IInitializableService
{
    List<string> IgnoreStatIds { get; }

    List<string> IncursionRoomStatIds { get; }

    List<string> LogbookFactionStatIds { get; }

    List<string> FireWeaponDamageIds { get; }

    List<string> ColdWeaponDamageIds { get; }

    List<string> LightningWeaponDamageIds { get; }

    string ClusterJewelSmallPassiveCountStatId { get; }

    string ClusterJewelSmallPassiveGrantStatId { get; }

    Dictionary<int, string> ClusterJewelSmallPassiveGrantOptions { get; }

    Task<List<TradeStatCategory>> GetList();
}
