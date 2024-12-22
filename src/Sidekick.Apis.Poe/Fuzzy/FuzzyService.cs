using System.Text.RegularExpressions;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Fuzzy;

public class FuzzyService(IGameLanguageProvider gameLanguageProvider) : IFuzzyService
{
    private readonly Regex CleanFuzzyPattern = new("[-+0-9%#]");
    private readonly Regex TrimPattern = new(@"\s+");

    public string CleanFuzzyText(string text)
    {
        text = CleanFuzzyPattern.Replace(text, string.Empty);
        return TrimPattern.Replace(text, " ").Trim();
    }

}
