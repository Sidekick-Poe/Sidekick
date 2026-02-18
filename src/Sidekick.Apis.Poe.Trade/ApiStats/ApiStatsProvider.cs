using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.ApiStats.Fuzzy;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;
using Sidekick.Data.Trade;
using Sidekick.Data.Trade.Models;
namespace Sidekick.Apis.Poe.Trade.ApiStats;

public class ApiStatsProvider
(
    TradeDataProvider tradeDataProvider,
    IInvariantStatsProvider invariantStatsProvider,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService,
    IFuzzyService fuzzyService
) : IApiStatsProvider
{
    private readonly Regex parseHashPattern = new("\\#");

    private readonly Regex newLinePattern = new(@"(?:\\)*[\r\n]+");
    private readonly Regex hashPattern = new(@"\\#");
    private readonly Regex parenthesesPattern = new(@"((?:\\\ )*\\\([^\(\)]*\\\))");

    private record StatReplaceEntry(Regex Pattern, string Replacement);

    private List<StatReplaceEntry> ReplacementPatterns { get; } = [];

    public Dictionary<StatCategory, List<StatDefinition>> Definitions { get; private set; } = new();

    /// <inheritdoc/>
    public int Priority => 200;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        var apiCategories = await tradeDataProvider.GetStats(game, gameLanguageProvider.Language.Code);

        ReplacementPatterns.Clear();
        if (!string.IsNullOrEmpty(gameLanguageProvider.Language.RegexIncreased))
        {
            ReplacementPatterns.Add(new(
                                    new Regex(gameLanguageProvider.Language.RegexIncreased),
                                    $"(?:{gameLanguageProvider.Language.RegexIncreased}|{gameLanguageProvider.Language.RegexReduced})"));
        }

        if (!string.IsNullOrEmpty(gameLanguageProvider.Language.RegexMore))
        {
            ReplacementPatterns.Add(new(
                                    new Regex(gameLanguageProvider.Language.RegexMore),
                                    $"(?:{gameLanguageProvider.Language.RegexMore}|{gameLanguageProvider.Language.RegexLess})"));
        }

        if (!string.IsNullOrEmpty(gameLanguageProvider.Language.RegexFaster))
        {
            ReplacementPatterns.Add(new(
                                    new Regex(gameLanguageProvider.Language.RegexFaster),
                                    $"(?:{gameLanguageProvider.Language.RegexFaster}|{gameLanguageProvider.Language.RegexSlower})"));
        }

        Definitions.Clear();
        foreach (var apiCategory in apiCategories)
        {
            var statCategory = GetStatCategory(apiCategory.Entries[0].Id);
            var patterns = ComputeCategoryPatterns(apiCategory, statCategory);

            if (!Definitions.TryAdd(statCategory, patterns))
            {
                Definitions[statCategory].AddRange(patterns);
            }
        }

        ComputeSecondaryDefinitions();

        // Prepare special pseudo patterns
        if (!Definitions.TryGetValue(StatCategory.Pseudo, out var pseudoPatterns))
        {
            pseudoPatterns = new List<StatDefinition>();
            Definitions.Add(StatCategory.Pseudo, pseudoPatterns);
        }

        var incursionPatterns = pseudoPatterns.Where(x => invariantStatsProvider.IncursionRoomStatIds.Contains(x.Id)).ToList();
        FillSpecialPseudoPattern(pseudoPatterns, incursionPatterns);

        var logbookPatterns = pseudoPatterns.Where(x => invariantStatsProvider.LogbookFactionStatIds.Contains(x.Id)).ToList();
        FillSpecialPseudoPattern(pseudoPatterns, logbookPatterns);
    }

    private List<StatDefinition> ComputeCategoryPatterns(TradeStatCategory apiCategory, StatCategory statCategory)
    {
        if (apiCategory.Entries.Count == 0 || statCategory == StatCategory.Undefined) return [];

        var patterns = new List<StatDefinition>();
        foreach (var entry in apiCategory.Entries)
        {
            patterns.AddRange(ComputeDefinition(statCategory, entry));
        }

        return patterns;
    }

    private IEnumerable<StatDefinition> ComputeDefinition(StatCategory statCategory, TradeStat entry)
    {
        if (invariantStatsProvider.IgnoreStatIds.Contains(entry.Id)) yield break;

        entry.Text = entry.Text.RemoveSquareBrackets();

        if (entry.Option?.Options.Count > 0)
        {
            foreach (var option in entry.Option.Options)
            {
                if (option.Text == null) continue;
                option.Text = option.Text.RemoveSquareBrackets();
                yield return new StatDefinition(statCategory, entry.Id, text: ComputeOptionText(entry.Text, option.Text), fuzzyText: ComputeFuzzyText(entry.Text, option.Text), pattern: ComputePattern(entry.Text, statCategory, option.Text))
                {
                    OptionId = option.Id,
                    OptionText = option.Text,
                };
            }
        }
        else
        {
            yield return new StatDefinition(statCategory, entry.Id, entry.Text, fuzzyText: ComputeFuzzyText(entry.Text), pattern: ComputePattern(entry.Text, statCategory));
        }
    }

    /// <inheritdoc/>
    public StatCategory GetStatCategory(string? apiId)
    {
        var value = apiId?.Split('.').First();
        return value.GetEnumFromValue<StatCategory>();
    }

    private void FillSpecialPseudoPattern(List<StatDefinition> pseudoPatterns, List<StatDefinition> patterns)
    {
        var specialPatterns = new List<StatDefinition>();
        foreach (var group in patterns.GroupBy(x => x.Id))
        {
            var pattern = group.OrderBy(x => x.OptionId).First();
            specialPatterns.Add(new StatDefinition(pattern.Category, pattern.Id, pattern.Text, fuzzyText: ComputeFuzzyText(pattern.Text), pattern: ComputePattern(pattern.Text.Split(':', 2).Last().Trim(), StatCategory.Pseudo))
            {
                OptionText = pattern.OptionText,
                OptionId = pattern.OptionId,
            });
        }

        var ids = specialPatterns.Select(x => x.Id).Distinct().ToList();
        pseudoPatterns.RemoveAll(x => ids.Contains(x.Id));
        pseudoPatterns.AddRange(specialPatterns);
    }

    private Regex ComputePattern(string text, StatCategory? category = null, string? optionText = null)
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

    private string ComputeFuzzyText(string text, string? optionText = null)
    {
        text = text.RemoveSquareBrackets();
        if (optionText != null) optionText = optionText.RemoveSquareBrackets();

        var fuzzyValue = text;

        if (string.IsNullOrEmpty(optionText))
        {
            return fuzzyService.CleanFuzzyText(fuzzyValue);
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

        return fuzzyService.CleanFuzzyText(fuzzyValue);
    }

    private string ComputeOptionText(string text, string optionText)
    {
        var optionLines = new List<string>();
        foreach (var optionLine in newLinePattern.Split(optionText))
        {
            optionLines.Add(parseHashPattern.Replace(text, optionLine));
        }

        return string.Join('\n', optionLines).Trim('\r', '\n');
    }

    public bool IsMatch(string id, string text)
    {
        foreach (var patternGroup in Definitions)
        {
            var pattern = patternGroup.Value.FirstOrDefault(x => x.Id == id);
            if (pattern != null && pattern.Pattern.IsMatch(text))
            {
                return true;
            }
        }

        return false;
    }

    private void ComputeSecondaryDefinitions()
    {
        var explicitDefinitions = Definitions.GetValueOrDefault(StatCategory.Explicit);
        if (explicitDefinitions == null) return;

        foreach (var group in Definitions)
        {
            if (!group.Key.HasExplicitStat()) continue;

            foreach (var definition in group.Value)
            {
                definition.SecondaryDefinitions.AddRange(explicitDefinitions.Where(x => x.Text == definition.Text));

            }
        }
    }

}
