using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo;

public interface IPseudoParser : IInitializableService
{
    List<PseudoModifier> Parse(List<ModifierLine> modifiers);
}
