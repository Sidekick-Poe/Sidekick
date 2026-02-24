using System.Text.RegularExpressions;
using Romanization;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Languages.Implementations;
namespace Sidekick.Data.Fuzzy;

public class FuzzyService : IFuzzyService
{
    private readonly Regex cleanFuzzyPattern = new("[-+0-9%#]");
    private readonly Regex trimPattern = new(@"\s+");

    public string CleanFuzzyText(IGameLanguage language, string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        text = cleanFuzzyPattern.Replace(text, string.Empty);
        text = trimPattern.Replace(text, " ").Trim();

        if (language is GameLanguageKo) text = RomanizeKorean(text);
        if (language is GameLanguageJa) text = RomanizeJapanese(text);

        return text;
    }

    private static Korean.RevisedRomanization? KoreanRomanization { get; set; }

    private static string RomanizeKorean(string text)
    {
        KoreanRomanization ??= new Korean.RevisedRomanization();
        try
        {
            return KoreanRomanization.Process(text);
        }
        catch (Exception)
        {
            return text;
        }
    }

    private static Japanese.KanjiReadings? JapaneseRomanization { get; set; }

    private static string RomanizeJapanese(string text)
    {
        JapaneseRomanization ??= new Japanese.KanjiReadings();
        try
        {
            return JapaneseRomanization.Process(text);
        }
        catch (Exception)
        {
            return text;
        }
    }

}
