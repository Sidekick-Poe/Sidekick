using Sidekick.Apis.Poe.Modifiers.Models;

namespace Sidekick.Apis.Poe.Parser.Modifiers;

public class ModifierMatch
(
    ParsingBlock block,
    IEnumerable<ParsingLine> lines,
    IEnumerable<ModifierDefinition> patterns
)
{
    public ParsingBlock Block { get; } = block;

    public IEnumerable<ParsingLine> Lines { get; } = lines;

    public IEnumerable<ModifierDefinition> Patterns { get; } = patterns;
}
