using System.Text.RegularExpressions;
using Sidekick.Common.Enums;
using Sidekick.Data.Extensions;
using Sidekick.Data.Fuzzy;
using Sidekick.Data.Items.Models;
using Sidekick.Data.Languages;
using Sidekick.Data.Trade;
using Sidekick.Data.Trade.Models;
using Sidekick.Data.Trade.Models.Raw;
namespace Sidekick.Data.Builder.Trade;

public class TradeStatBuilder(
    TradeDataProvider tradeDataProvider,
    IFuzzyService fuzzyService,
    DataProvider dataProvider)
{
    private record StatReplaceEntry(Regex Pattern, string Replacement);

    private readonly Regex parseHashPattern = new("\\#");
    private readonly Regex newLinePattern = new(@"(?:\\)*[\r\n]+");
    private readonly Regex hashPattern = new(@"\\#");
    private readonly Regex parenthesesPattern = new(@"((?:\\\ )*\\\([^\(\)]*\\\))");

    private List<StatReplaceEntry> ReplacementPatterns { get; set; } = [];

    public async Task Build(IGameLanguage language)
    {
        await Build(GameType.PathOfExile1, language);
        await Build(GameType.PathOfExile2, language);
    }

    private async Task Build(GameType game, IGameLanguage language)
    {
        var apiCategories = await tradeDataProvider.GetRawStats(game, language.Code);
        var invariantStats = await dataProvider.Read<TradeInvariantStats>(game, $"trade/stats.invariant.json");

        ReplacementPatterns = BuildReplacementPatterns(language);
        var definitions = new List<TradeStatDefinition>();

        foreach (var apiCategory in apiCategories)
        {
            var statCategory = GetStatCategory(apiCategory.Entries[0].Id);
            if (apiCategory.Entries.Count == 0 || statCategory == StatCategory.Undefined) continue;

            foreach (var entry in apiCategory.Entries)
            {
                if (invariantStats.IgnoreStatIds.Contains(entry.Id)) continue;
                if (string.IsNullOrEmpty(entry.Id)) continue;

                definitions.AddRange(GetDefinitions(language, statCategory, entry));
            }
        }

        ComputeSecondaryDefinitions();

        ComputeSpecialPseudoPattern(invariantStats.IncursionRoomStatIds);
        ComputeSpecialPseudoPattern(invariantStats.LogbookFactionStatIds);

        await dataProvider.Write(game, $"trade/stats.{language.Code}.json", definitions);

        return;

        void ComputeSecondaryDefinitions()
        {
            var explicitDefinitions = definitions.Where(x => x.Category == StatCategory.Explicit).ToList();

            foreach (var definition in definitions)
            {
                if (!definition.Category.HasExplicitStat()) continue;

                definition.SecondaryDefinitions ??= [];
                definition.SecondaryDefinitions.AddRange(explicitDefinitions
                                                             .Where(x => x.Text == definition.Text)
                                                             .Select(x => x.Id));
            }
        }

        void ComputeSpecialPseudoPattern(List<string> patternIds)
        {
            var patterns = definitions
                .Where(x => x.Category == StatCategory.Pseudo)
                .Where(x => patternIds.Contains(x.Id))
                .ToList();

            var specialPatterns = new List<TradeStatDefinition>();
            foreach (var group in patterns.GroupBy(x => x.Id))
            {
                var pattern = group.OrderBy(x => x.OptionId).First();
                specialPatterns.Add(new TradeStatDefinition()
                {
                    Category = pattern.Category,
                    Id = pattern.Id,
                    Text = GetText(pattern.Text),
                    FuzzyText = GetFuzzyText(language, pattern.Text),
                    Pattern = GetPattern(pattern.Text.Split(':', 2).Last().Trim(), StatCategory.Pseudo),
                    OptionText = pattern.OptionText,
                    OptionId = pattern.OptionId,
                });
            }

            var ids = specialPatterns.Select(x => x.Id).Distinct().ToList();
            definitions.RemoveAll(x => ids.Contains(x.Id));
            definitions.AddRange(specialPatterns);
        }
    }

    private List<StatReplaceEntry> BuildReplacementPatterns(IGameLanguage language)
    {
        List<StatReplaceEntry> result = [];
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

    private StatCategory GetStatCategory(string? apiId)
    {
        var value = apiId?.Split('.').First();
        return value.GetEnumFromValue<StatCategory>();
    }

    private IEnumerable<TradeStatDefinition> GetDefinitions(IGameLanguage language, StatCategory statCategory, RawTradeStat entry)
    {
        entry.Text = entry.Text.RemoveSquareBrackets();

        if (entry.Options?.Options.Count > 0)
        {
            foreach (var option in entry.Options.Options)
            {
                option.Text = option.Text.RemoveSquareBrackets();
                yield return new TradeStatDefinition()
                {
                    Category = statCategory,
                    Id = entry.Id,
                    Text = GetText(entry.Text, option.Text),
                    FuzzyText = GetFuzzyText(language, entry.Text, option.Text),
                    Pattern = GetPattern(entry.Text, statCategory, option.Text),
                    OptionId = option.Id,
                    OptionText = option.Text,
                };
            }
        }
        else
        {
            yield return new TradeStatDefinition()
            {
                Category = statCategory,
                Id = entry.Id,
                Text = GetText(entry.Text),
                FuzzyText = GetFuzzyText(language, entry.Text),
                Pattern = GetPattern(entry.Text, statCategory),
            };
        }
    }

    private string GetText(string text, string? optionText = null)
    {
        if (optionText == null) return text;

        var optionLines = new List<string>();
        foreach (var optionLine in newLinePattern.Split(optionText))
        {
            optionLines.Add(parseHashPattern.Replace(text, optionLine));
        }

        return string.Join('\n', optionLines).Trim('\r', '\n');
    }

    private string GetFuzzyText(IGameLanguage language, string text, string? optionText = null)
    {
        text = text.RemoveSquareBrackets();
        if (optionText != null) optionText = optionText.RemoveSquareBrackets();

        var fuzzyValue = text;

        if (string.IsNullOrEmpty(optionText))
        {
            return fuzzyService.CleanFuzzyText(language, fuzzyValue);
        }

        foreach (var optionLine in newLinePattern.Split(optionText))
        {
            if (parseHashPattern.IsMatch(fuzzyValue))
            {
                fuzzyValue = parseHashPattern.Replace(fuzzyValue, optionLine);
            }
            else
            {
                fuzzyValue += optionLine;
            }
        }

        return fuzzyService.CleanFuzzyText(language, fuzzyValue);
    }

    private Regex GetPattern(string text, StatCategory? category = null, string? optionText = null)
    {
        text = text.RemoveSquareBrackets();
        if (optionText != null) optionText = optionText.RemoveSquareBrackets();

        // The notes in parentheses are never translated by the game.
        // We should be fine hardcoding them this way.
        var suffix = category switch
        {
            StatCategory.Enchant => @"\ \(enchant\)",
            StatCategory.Rune => @"\ \(rune\)",
            StatCategory.Implicit => @"\ \(implicit\)",
            StatCategory.Veiled => @"\ \(veiled\)",
            StatCategory.Scourge => @"\ \(scourge\)",
            StatCategory.Crucible => @"\ \(crucible\)",
            StatCategory.Crafted => @"\ \(crafted\)",
            StatCategory.Fractured => @"\ \(fractured\)",
            StatCategory.Desecrated => @"\ \(desecrated\)",
            StatCategory.Explicit => @"(?:\ \(mutated\))?",
            _ => "",
        };

        var patternValue = Regex.Escape(text);
        patternValue = parenthesesPattern.Replace(patternValue, "(?:$1)?");
        patternValue = newLinePattern.Replace(patternValue, "\\n");

        foreach (var replacement in ReplacementPatterns)
        {
            patternValue = replacement.Pattern.Replace(patternValue, replacement.Replacement);
        }

        if (string.IsNullOrEmpty(optionText))
        {
            patternValue = hashPattern.Replace(patternValue, "([-+0-9,.]+)") + suffix;
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

        // For multiline stats, the category can be suffixed on all lines.
        patternValue = patternValue.Replace("\\n", suffix + "\\n");

        return new Regex($"^{patternValue}$", RegexOptions.None);
    }

}
