using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Parser.Pseudo
{
    public interface IPseudoParser : IInitializableService
    {
        List<PseudoModifier> Parse(List<ModifierLine> modifiers);
    }
}
