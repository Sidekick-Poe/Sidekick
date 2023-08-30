namespace Sidekick.Apis.Poe.Modifiers.Models
{
    public class FuzzyResult
    {
        public FuzzyResult(
            int ratio,
            List<FuzzyEntry> entries)
        {
            Ratio = ratio;
            Entries = entries;
        }

        public int Ratio { get; }

        public List<FuzzyEntry> Entries { get; }
    }
}
