using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Requirements;

public class RequirementsParser(IGameLanguageProvider gameLanguageProvider) : IRequirementsParser
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionRequirements.ToRegexLine();

    public void Parse(TextItem textItem)
    {
        foreach (var block in textItem.Blocks.Where(x => !x.Parsed))
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
