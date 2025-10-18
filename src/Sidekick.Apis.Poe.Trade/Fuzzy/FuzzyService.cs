using System.Text.RegularExpressions;
using Romanization;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Languages.Implementations;

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

        if (gameLanguageProvider.Language is GameLanguageKo) text = RomanizeKorean(text);
        if (gameLanguageProvider.Language is GameLanguageJa) text = RomanizeJapanese(text);
        
        return text;
    }

    private static Korean.RevisedRomanization? KoreanRomanization { get; set; }

    private static string RomanizeKorean(string text)
    {
        KoreanRomanization ??= new Korean.RevisedRomanization();
        try
        {
            text = KoreanRomanization.Process(text);
        }
        catch (Exception)
        {
            // Do nothing if the romanization fails.
        }

        return text;
    }

    private static Japanese.KanjiReadings? JapaneseRomanization { get; set; }

    private static string RomanizeJapanese(string text)
    {
        JapaneseRomanization ??= new Japanese.KanjiReadings();
        try
        {
            text = JapaneseRomanization.Process(text);
        }
        catch (Exception)
        {
            // Do nothing if the romanization fails.
        }

        return text;
    }

}
