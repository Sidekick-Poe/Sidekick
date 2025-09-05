using Sidekick.Apis.Poe.Trade.Modifiers.Models;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Modifiers;

public interface IInvariantModifierProvider : IInitializableService
{
    Dictionary<ModifierCategory, List<ApiModifier>> Categories { get; }

    List<string> IgnoreModifierIds { get; }

    List<string> IncursionRoomModifierIds { get; }

    List<string> LogbookFactionModifierIds { get; }

    List<string> FireWeaponDamageIds { get; }

    List<string> ColdWeaponDamageIds { get; }

    List<string> LightningWeaponDamageIds { get; }

    string ClusterJewelSmallPassiveCountModifierId { get; }

    string ClusterJewelSmallPassiveGrantModifierId { get; }

    Dictionary<int, string> ClusterJewelSmallPassiveGrantOptions { get; }

    Dictionary<Type, string> PseudoModifierIds { get; }
}
