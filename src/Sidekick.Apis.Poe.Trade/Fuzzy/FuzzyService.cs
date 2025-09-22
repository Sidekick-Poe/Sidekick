using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Languages;

namespace Sidekick.Apis.Poe.Trade.Fuzzy;

public class FuzzyService : IFuzzyService
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
        return text;
    }

}
