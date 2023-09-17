using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Pseudo
{
    public interface IPseudoModifierProvider : IInitializableService
    {
        List<PseudoModifier> Parse(List<ModifierLine> modifiers);
    }
}
