using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Modifiers;

public interface IInvariantModifierProvider : IInitializableService
{
    List<string> IgnoreModifierIds { get; }

    List<string> IncursionRoomModifierIds { get; }

    List<string> LogbookFactionModifierIds { get; }

    string ClusterJewelSmallPassiveCountModifierId { get; }

    string ClusterJewelSmallPassiveGrantModifierId { get; }

    Dictionary<int, string> ClusterJewelSmallPassiveGrantOptions { get; }

    Task<List<ApiCategory>> GetList();
}
