using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Modifiers.Models;

namespace Sidekick.Apis.Poe.Trade.Parser.Modifiers;

public class ModifierMatch
(
    TextBlock block,
    IEnumerable<TextLine> lines,
    IEnumerable<ModifierDefinition> definitions
)
{
    public TextBlock Block { get; } = block;

    public IEnumerable<TextLine> Lines { get; } = lines;

    public IEnumerable<ModifierDefinition> Definitions { get; } = definitions;
}
