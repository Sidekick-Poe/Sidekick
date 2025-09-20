using Sidekick.Apis.Poe.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo;

public interface IPseudoParser : IInitializableService
{
    List<PseudoModifier> Parse(List<ModifierLine> modifiers);
}
