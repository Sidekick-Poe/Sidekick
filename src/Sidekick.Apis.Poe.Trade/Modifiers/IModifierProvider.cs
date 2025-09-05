using Sidekick.Apis.Poe.Trade.Modifiers.Models;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Modifiers;

public interface IModifierProvider : IInitializableService
{
    bool IsMatch(string id, string text);

    Dictionary<ModifierCategory, List<ModifierDefinition>> Definitions { get; }

    ModifierDefinition? GetById(string? id);

}
