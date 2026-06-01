using System.Text.RegularExpressions;
using Sidekick.Data.Extensions;
using Sidekick.Data.Fuzzy;
using Sidekick.Data.Languages;
namespace Sidekick.Data.Builder.Stats;

public class StatPatternBuilder
{
    private readonly IGameLanguage language;
    private readonly IFuzzyService fuzzyService;

    private record TradeReplaceEntry(Regex Pattern, string Replacement);

    private readonly Regex textHashPattern = new(@"\#", RegexOptions.Compiled);
    private readonly Regex textGameHashPattern = new(@"\{\d+}", RegexOptions.Compiled);
    private readonly Regex textLocalPattern = new(@" \(Local\)$", RegexOptions.Compiled);

    private readonly Regex newLinePattern = new(@"(?:\\)*[\r\n]+", RegexOptions.Compiled);
    private readonly Regex hashPattern = new(@"\\#", RegexOptions.Compiled);
    private readonly Regex gameHashPattern = new(@"\\{\d+}", RegexOptions.Compiled);
    private readonly Regex parenthesesPattern = new(@"(\\\ *\\\([^\(\)]*\\\))", RegexOptions.Compiled);

    private List<TradeReplaceEntry> TradeReplacePatterns { get; }

    public StatPatternBuilder(IGameLanguage language, IFuzzyService fuzzyService)
    {
        this.language = language;
        this.fuzzyService = fuzzyService;
        TradeReplacePatterns = BuildReplacementPatterns();

        return;

        List<TradeReplaceEntry> BuildReplacementPatterns()
        {
            List<TradeReplaceEntry> result = [];
            if (!string.IsNullOrEmpty(language.RegexIncreased))
            {
                result.Add(new(
                           new Regex(language.RegexIncreased),
                           $"(?:{language.RegexIncreased}|{language.RegexReduced})"));
            }

            if (!string.IsNullOrEmpty(language.RegexMore))
            {
                result.Add(new(
                           new Regex(language.RegexMore),
                           $"(?:{language.RegexMore}|{language.RegexLess})"));
            }

            if (!string.IsNullOrEmpty(language.RegexFaster))
            {
                result.Add(new(
                           new Regex(language.RegexFaster),
                           $"(?:{language.RegexFaster}|{language.RegexSlower})"));
            }

            return result;
        }
    }

    public Regex GetPattern(string text, string? optionText = null, bool replacePatterns = false)
    {
        text = text.RemoveSquareBrackets();
        text = textLocalPattern.Replace(text, string.Empty);

        var suffix = @"(?:\ \([a-z]+\))?";

        var patternValue = Regex.Escape(text);
        patternValue = newLinePattern.Replace(patternValue, @"\n");
        patternValue = parenthesesPattern.Replace(patternValue, "(?:$1)?");

        if (string.IsNullOrEmpty(optionText))
        {
            patternValue = hashPattern.Replace(patternValue, @"([-+0-9,.]+)");
            patternValue = gameHashPattern.Replace(patternValue, @"([-+0-9,.]+)");
        }
        else
        {
            var optionLines = new List<string>();
            foreach (var optionLine in newLinePattern.Split(optionText))
            {
                optionLines.Add(hashPattern.Replace(patternValue, Regex.Escape(optionLine)) + suffix);
            }

            patternValue = string.Join('\n', optionLines.Where(x => !string.IsNullOrEmpty(x)));
        }

        if (replacePatterns)
        {
            foreach (var replaceEntry in TradeReplacePatterns)
            {
                patternValue = replaceEntry.Pattern.Replace(patternValue, replaceEntry.Replacement);
            }
        }

        patternValue = patternValue.Replace(@"\n", suffix + @"\n");// For multiline stats, the category can be suffixed on all lines.
        patternValue += suffix;

        return new Regex($"^{patternValue}$", RegexOptions.Compiled);
    }

    public string GetText(string text, string? optionText = null)
    {
        text = text.RemoveSquareBrackets();
        text = textGameHashPattern.Replace(text, "#");
        text = textLocalPattern.Replace(text, string.Empty);
        if (optionText == null) return text;

        optionText = optionText.RemoveSquareBrackets();

        var optionLines = new List<string>();
        foreach (var optionLine in newLinePattern.Split(optionText))
        {
            optionLines.Add(textHashPattern.Replace(text, optionLine));
        }

        return string.Join('\n', optionLines).Trim('\r', '\n');
    }

    public string GetFuzzyText(string text, string? optionText = null)
    {
        if (string.IsNullOrEmpty(optionText))
        {
            return fuzzyService.CleanFuzzyText(language, text);
        }

        foreach (var optionLine in newLinePattern.Split(optionText))
        {
            text = textHashPattern.Replace(text, optionLine);
        }

        return fuzzyService.CleanFuzzyText(language, text);
    }
}
