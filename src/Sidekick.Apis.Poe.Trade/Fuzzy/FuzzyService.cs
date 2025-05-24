using System.Text.RegularExpressions;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Trade.Fuzzy;

public class FuzzyService(IGameLanguageProvider gameLanguageProvider) : IFuzzyService
{
    private readonly Regex cleanFuzzyPattern = new("[-+0-9%#]");
    private readonly Regex trimPattern = new(@"\s+");

    public string CleanFuzzyText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        text = cleanFuzzyPattern.Replace(text, string.Empty);
        text = trimPattern.Replace(text, " ").Trim();
        text = gameLanguageProvider.Language.GetFuzzyText(text) ?? string.Empty;
        return text;
    }

}
