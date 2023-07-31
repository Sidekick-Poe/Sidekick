using System.Collections.Generic;

namespace Sidekick.Apis.Poe.Modifiers.Models
{
    public class FuzzyResult
    {
        public int Ratio { get; set; }

        public List<FuzzyEntry> Entries { get; set; }
    }
}
