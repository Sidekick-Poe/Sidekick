namespace Sidekick.Apis.Poe.Modifiers.Models
{
    public class FuzzyEntry
    {
        public FuzzyEntry(
            ModifierPatternMetadata metadata,
            ModifierPattern pattern)
        {
            Metadata = metadata;
            Pattern = pattern;
        }

        public ModifierPatternMetadata Metadata { get; }

        public ModifierPattern Pattern { get; }
    }
}
