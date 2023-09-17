namespace Sidekick.Apis.Poe.Modifiers.Models
{
    public class FuzzyResult
    {
        public FuzzyResult(
            int ratio,
            List<ModifierPattern> patterns)
        {
            Ratio = ratio;
            Patterns = patterns;
        }

        public int Ratio { get; }

        public List<ModifierPattern> Patterns { get; }
    }
}
