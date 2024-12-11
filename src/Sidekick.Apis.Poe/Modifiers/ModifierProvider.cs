using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Modifiers;

public class ModifierProvider
(
    ICacheProvider cacheProvider,
    IPoeTradeClient poeTradeClient,
    IInvariantModifierProvider invariantModifierProvider,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService
) : IModifierProvider
{
    private readonly Regex ParseHashPattern = new("\\#");
    private readonly Regex NewLinePattern = new("(?:\\\\)*[\\r\\n]+");
    private readonly Regex HashPattern = new("\\\\#");
    private readonly Regex ParenthesesPattern = new("((?:\\\\\\ )*\\\\\\([^\\(\\)]*\\\\\\))");
    private readonly Regex CleanFuzzyPattern = new("[-+0-9%#]");
    private readonly Regex TrimPattern = new(@"\s+");

    public Dictionary<ModifierCategory, List<ModifierPattern>> Patterns { get; } = new();

    public Dictionary<string, List<ModifierPattern>> FuzzyDictionary { get; } = new();

    /// <inheritdoc/>
    public int Priority => 200;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();
        var cacheKey = $"{game.GetValueAttribute()}_Modifiers";
        var apiCategories = await cacheProvider.GetOrSet(cacheKey, () => poeTradeClient.Fetch<ApiCategory>(game, gameLanguageProvider.Language, "data/stats"));

        foreach (var apiCategory in apiCategories.Result)
        {
            if (apiCategory.Entries.Count == 0)
            {
                continue;
            }

            var modifierCategory = GetModifierCategory(apiCategory.Entries[0].Id);
            if (modifierCategory == ModifierCategory.Undefined)
            {
                continue;
            }

            var patterns = new List<ModifierPattern>();
            foreach (var entry in apiCategory.Entries)
            {
                var isOption = entry.Option?.Options.Any() ?? false;
                if (isOption)
                {
                    for (var i = 0; i < entry.Option?.Options.Count; i++)
                    {
                        var optionText = entry.Option.Options[i].Text;
                        if (optionText == null)
                        {
                            continue;
                        }

                        patterns.Add(new ModifierPattern(category: modifierCategory, id: entry.Id, isOption: entry.Option?.Options?.Any() ?? false, text: ComputeOptionText(entry.Text, optionText), fuzzyText: ComputeFuzzyText(modifierCategory, entry.Text, optionText), pattern: ComputePattern(entry.Text, modifierCategory, optionText))
                        {
                            Value = entry.Option?.Options[i].Id,
                        });
                    }
                }
                else
                {
                    patterns.Add(new ModifierPattern(category: modifierCategory, id: entry.Id, isOption: entry.Option?.Options?.Any() ?? false, text: entry.Text, fuzzyText: ComputeFuzzyText(modifierCategory, entry.Text), pattern: ComputePattern(entry.Text, modifierCategory)));
                }
            }

            if (Patterns.ContainsKey(modifierCategory))
            {
                Patterns[modifierCategory].AddRange(patterns);
            }
            else
            {
                Patterns.Add(modifierCategory, patterns);
            }

            BuildFuzzyDictionary(patterns);
        }

        // Prepare special pseudo patterns
        var pseudoPatterns = Patterns.GetValueOrDefault(ModifierCategory.Pseudo) ?? [];

        var incursionPatterns = pseudoPatterns.Where(x => invariantModifierProvider.IncursionRoomModifierIds.Contains(x.Id)).ToList();
        ComputeSpecialPseudoPattern(pseudoPatterns, incursionPatterns);

        var logbookPatterns = pseudoPatterns.Where(x => invariantModifierProvider.LogbookFactionModifierIds.Contains(x.Id)).ToList();
        ComputeSpecialPseudoPattern(pseudoPatterns, logbookPatterns);
    }

    /// <inheritdoc/>
    public ModifierCategory GetModifierCategory(string? apiId) => apiId?.Split('.').First() switch
    {
        "crafted" => ModifierCategory.Crafted,
        "delve" => ModifierCategory.Delve,
        "enchant" => ModifierCategory.Enchant,
        "explicit" => ModifierCategory.Explicit,
        "fractured" => ModifierCategory.Fractured,
        "implicit" => ModifierCategory.Implicit,
        "monster" => ModifierCategory.Monster,
        "pseudo" => ModifierCategory.Pseudo,
        "scourge" => ModifierCategory.Scourge,
        "veiled" => ModifierCategory.Veiled,
        "crucible" => ModifierCategory.Crucible,
        _ => ModifierCategory.Undefined,
    };

    private void ComputeSpecialPseudoPattern(List<ModifierPattern> pseudoPatterns, List<ModifierPattern> patterns)
    {
        var specialPatterns = new List<ModifierPattern>();
        foreach (var group in patterns.GroupBy(x => x.Id))
        {
            var pattern = group.OrderBy(x => x.Value).First();
            specialPatterns.Add(new ModifierPattern(category: pattern.Category, id: pattern.Id, isOption: pattern.IsOption, text: pattern.Text, fuzzyText: ComputeFuzzyText(ModifierCategory.Pseudo, pattern.Text), pattern: ComputePattern(pattern.Text.Split(':', 2).Last().Trim(), ModifierCategory.Pseudo))
            {
                OptionText = pattern.OptionText,
                Value = pattern.Value,
            });
        }

        var ids = specialPatterns.Select(x => x.Id).Distinct().ToList();
        pseudoPatterns.RemoveAll(x => ids.Contains(x.Id));
        pseudoPatterns.AddRange(specialPatterns);
    }

    private Regex ComputePattern(string text, ModifierCategory? category = null, string? optionText = null)
    {
        // The notes in parentheses are never translated by the game.
        // We should be fine hardcoding them this way.
        var suffix = category switch
        {
            ModifierCategory.Implicit => "(?:\\ \\(implicit\\))",
            ModifierCategory.Enchant => "(?:\\ \\(enchant\\))",
            ModifierCategory.Crafted => "(?:\\ \\(crafted\\))?",
            ModifierCategory.Veiled => "(?:\\ \\(veiled\\))",
            ModifierCategory.Fractured => "(?:\\ \\(fractured\\))?",
            ModifierCategory.Scourge => "(?:\\ \\(scourge\\))",
            ModifierCategory.Crucible => "(?:\\ \\(crucible\\))",
            ModifierCategory.Explicit => "(?:\\ \\((?:crafted|fractured)\\))?",
            _ => "",
        };

        var patternValue = Regex.Escape(text);
        patternValue = ParenthesesPattern.Replace(patternValue, "(?:$1)?");
        patternValue = NewLinePattern.Replace(patternValue, "\\n");

        if (string.IsNullOrEmpty(optionText))
        {
            patternValue = HashPattern.Replace(patternValue, "[-+0-9,.]+") + suffix;
        }
        else
        {
            var optionLines = new List<string>();
            foreach (var optionLine in NewLinePattern.Split(optionText))
            {
                optionLines.Add(HashPattern.Replace(patternValue, Regex.Escape(optionLine)) + suffix);
            }

            patternValue = string.Join('\n', optionLines);
        }

        return new Regex($"^{patternValue}$", RegexOptions.None);
    }

    private string ComputeFuzzyText(ModifierCategory category, string text, string? optionText = null)
    {
        var fuzzyValue = text;

        if (!string.IsNullOrEmpty(optionText))
        {
            foreach (var optionLine in NewLinePattern.Split(optionText))
            {
                if (ParseHashPattern.IsMatch(fuzzyValue))
                {
                    fuzzyValue = ParseHashPattern.Replace(fuzzyValue, optionLine);
                }
                else
                {
                    fuzzyValue += optionLine;
                }
            }
        }

        // Add the suffix
        // The notes in parentheses are never translated by the game.
        // We should be fine hardcoding them this way.
        fuzzyValue += category switch
        {
            ModifierCategory.Implicit => " (implicit)",
            ModifierCategory.Enchant => " (enchant)",
            ModifierCategory.Crafted => " (crafted)",
            ModifierCategory.Veiled => " (veiled)",
            ModifierCategory.Fractured => " (fractured)",
            ModifierCategory.Scourge => " (scourge)",
            ModifierCategory.Pseudo => " (pseudo)",
            ModifierCategory.Crucible => " (crucible)",
            _ => "",
        };

        return CleanFuzzyText(fuzzyValue);
    }

    private string CleanFuzzyText(string text)
    {
        text = CleanFuzzyPattern.Replace(text, string.Empty);
        return TrimPattern.Replace(text, " ").Trim();
    }

    private void BuildFuzzyDictionary(List<ModifierPattern> patterns)
    {
        foreach (var pattern in patterns)
        {
            if (!FuzzyDictionary.ContainsKey(pattern.FuzzyText))
            {
                FuzzyDictionary.Add(pattern.FuzzyText, new());
            }

            FuzzyDictionary[pattern.FuzzyText].Add(pattern);
        }
    }

    private string ComputeOptionText(string text, string optionText)
    {
        var optionLines = new List<string>();
        foreach (var optionLine in NewLinePattern.Split(optionText))
        {
            optionLines.Add(ParseHashPattern.Replace(text, optionLine));
        }

        return string.Join('\n', optionLines).Trim('\r', '\n');
    }

    public bool IsMatch(string id, string text)
    {
        foreach (var patternGroup in Patterns)
        {
            var pattern = patternGroup.Value.FirstOrDefault(x => x.Id == id);
            if (pattern != null && pattern.Pattern.IsMatch(text))
            {
                return true;
            }
        }

        return false;
    }
}
