using Sidekick.Common.Game.Items.Modifiers;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Pseudo
{
    public interface IPseudoModifierProvider : IInitializableService
    {
        List<Modifier> Parse(List<ModifierLine> modifiers);
    }
}
