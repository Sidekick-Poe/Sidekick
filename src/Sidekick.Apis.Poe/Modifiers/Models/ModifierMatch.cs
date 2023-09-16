using Sidekick.Apis.Poe.Parser;

namespace Sidekick.Apis.Poe.Modifiers.Models
{
    public class ModifierMatch
    {
        public ModifierMatch(
            ParsingBlock block,
            IEnumerable<ParsingLine> lines,
            IEnumerable<ModifierPattern> patterns)
        {
            Block = block;
            Lines = lines;
            Patterns = patterns;
        }

        public ParsingBlock Block { get; }

        public IEnumerable<ParsingLine> Lines { get; }

        public IEnumerable<ModifierPattern> Patterns { get; }
    }
}
