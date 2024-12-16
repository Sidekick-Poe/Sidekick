using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Modifiers
{
    public interface IInvariantModifierProvider : IInitializableService
    {
        List<string> IncursionRoomModifierIds { get; }

        List<string> LogbookFactionModifierIds { get; }

        List<string> FireWeaponDamageIds { get; }

        List<string> ColdWeaponDamageIds { get; }

        List<string> LightningWeaponDamageIds { get; }

        string ClusterJewelSmallPassiveCountModifierId { get; }

        string ClusterJewelSmallPassiveGrantModifierId { get; }

        Dictionary<int, string> ClusterJewelSmallPassiveGrantOptions { get; }

        Task<List<ApiCategory>> GetList();
    }
}
