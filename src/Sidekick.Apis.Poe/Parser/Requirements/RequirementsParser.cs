using System.Text.RegularExpressions;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Requirements
{
    public class RequirementsParser(IGameLanguageProvider gameLanguageProvider) : IRequirementsParser
    {
        /// <inheritdoc/>
        public int Priority => 100;

        private Regex Pattern { get; set; } = null!;

        /// <inheritdoc/>
        public Task Initialize()
        {
            Pattern = gameLanguageProvider.Language.DescriptionRequirements.ToRegexLine();

            return Task.CompletedTask;
        }

        public void Parse(ParsingItem parsingItem)
        {
            foreach (var block in parsingItem.Blocks.Where(x => !x.Parsed))
            {
                if (!block.TryParseRegex(Pattern, out _))
                {
                    continue;
                }

                block.Parsed = true;
                return;
            }
        }
    }
}
