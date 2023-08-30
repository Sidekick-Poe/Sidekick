using System.Text.RegularExpressions;

namespace Sidekick.Apis.Poe.Pseudo.Models
{
    public class PseudoPatternGroup
    {
        public PseudoPatternGroup(
            Modifiers.Models.ApiCategory apiCategory,
            string id,
            params PseudoPattern[] patterns)
            : this(apiCategory, id, null, patterns)
        {
        }

        public PseudoPatternGroup(
            Modifiers.Models.ApiCategory apiCategory,
            string id,
            Regex? exception,
            params PseudoPattern[] patterns)
        {
            Id = id;
            Exception = exception;
            Patterns = patterns.ToList();

            Text = apiCategory.Entries.First(x => x.Id == id).Text;
        }

        public string Id { get; }
        public Regex? Exception { get; }
        public List<PseudoPattern> Patterns { get; }
        public string? Text { get; }

        public override string ToString()
        {
            if (Text == null)
            {
                return Id;
            }

            return $"{Text} - {Id}";
        }
    }
}
