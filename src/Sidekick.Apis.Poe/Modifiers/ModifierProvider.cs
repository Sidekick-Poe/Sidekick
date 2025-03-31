using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Fuzzy;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Common;
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
    ISettingsService settingsService,
    IFuzzyService fuzzyService
) : IModifierProvider
{
    private readonly Regex parseHashPattern = new("\\#");

    /// <summary>
    /// A regular expression used to extract and process text within square brackets,
    /// optionally separated by pipes, for parsing modifier patterns within game data.
    /// </summary>
    /// <example>
    /// [ItemRarity|Rarity of Items] => Rarity of Items
    /// [Spell] => Spell
    /// </example>
    private static readonly Regex parseSquareBracketPattern = new("\\[.*?\\|?([^\\|\\[\\]]*)\\]");

    public static string RemoveSquareBrackets(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        return parseSquareBracketPattern.Replace(text, "$1");
    }

    private readonly Regex newLinePattern = new("(?:\\\\)*[\\r\\n]+");
    private readonly Regex hashPattern = new("\\\\#");
    private readonly Regex parenthesesPattern = new("((?:\\\\\\ )*\\\\\\([^\\(\\)]*\\\\\\))");

    public Dictionary<ModifierCategory, List<ModifierDefinition>> Definitions { get; } = new();

    /// <inheritdoc/>
    public int Priority => 200;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        if (SidekickConfiguration.IsPoeApiDown) return;

        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();
        var cacheKey = $"{game.GetValueAttribute()}_Modifiers";
        var apiCategories = await cacheProvider.GetOrSet(cacheKey, () => poeTradeClient.Fetch<ApiCategory>(game, gameLanguageProvider.Language, "data/stats"), (cache) => cache.Result.Any());

        foreach (var apiCategory in apiCategories.Result)
        {
            var modifierCategory = GetModifierCategory(apiCategory.Entries[0].Id);
            var patterns = ComputeCategoryPatterns(apiCategory, modifierCategory);

            if (!Definitions.TryAdd(modifierCategory, patterns))
            {
                Definitions[modifierCategory].AddRange(patterns);
            }
        }

        // Prepare special pseudo patterns
        if (!Definitions.TryGetValue(ModifierCategory.Pseudo, out var pseudoPatterns))
        {
            pseudoPatterns = new List<ModifierDefinition>();
            Definitions.Add(ModifierCategory.Pseudo, pseudoPatterns);
        }

        var incursionPatterns = pseudoPatterns.Where(x => invariantModifierProvider.IncursionRoomModifierIds.Contains(x.ApiId)).ToList();
        FillSpecialPseudoPattern(pseudoPatterns, incursionPatterns);

        var logbookPatterns = pseudoPatterns.Where(x => invariantModifierProvider.LogbookFactionModifierIds.Contains(x.ApiId)).ToList();
        FillSpecialPseudoPattern(pseudoPatterns, logbookPatterns);
    }

    private List<ModifierDefinition> ComputeCategoryPatterns(ApiCategory apiCategory, ModifierCategory modifierCategory)
    {
        if (apiCategory.Entries.Count == 0 || modifierCategory == ModifierCategory.Undefined)
        {
            return [];
        }

        var patterns = new List<ModifierDefinition>();
        foreach (var entry in apiCategory.Entries)
        {
            if (invariantModifierProvider.IgnoreModifierIds.Contains(entry.Id)) continue;

            entry.Text = RemoveSquareBrackets(entry.Text);

            if (entry.Option?.Options.Count > 0)
            {
                foreach (var option in entry.Option.Options)
                {
                    if (option.Text == null) continue;
                    option.Text = RemoveSquareBrackets(option.Text);
                    patterns.Add(new ModifierDefinition(modifierCategory, entry.Id, apiText: ComputeOptionText(entry.Text, option.Text), fuzzyText: ComputeFuzzyText(modifierCategory, entry.Text, option.Text), pattern: ComputePattern(entry.Text, modifierCategory, option.Text))
                    {
                        OptionId = option.Id,
                    });
                }
            }
            else
            {
                patterns.Add(new ModifierDefinition(modifierCategory, entry.Id, entry.Text, fuzzyText: ComputeFuzzyText(modifierCategory, entry.Text), pattern: ComputePattern(entry.Text, modifierCategory)));
            }
        }

        return patterns;
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
        "rune" => ModifierCategory.Rune,
        "sanctum" => ModifierCategory.Sanctum,
        _ => ModifierCategory.Undefined,
    };

    private void FillSpecialPseudoPattern(List<ModifierDefinition> pseudoPatterns, List<ModifierDefinition> patterns)
    {
        var specialPatterns = new List<ModifierDefinition>();
        foreach (var group in patterns.GroupBy(x => x.ApiId))
        {
            var pattern = group.OrderBy(x => x.OptionId).First();
            specialPatterns.Add(new ModifierDefinition(pattern.Category, pattern.ApiId, pattern.ApiText, fuzzyText: ComputeFuzzyText(ModifierCategory.Pseudo, pattern.ApiText), pattern: ComputePattern(pattern.ApiText.Split(':', 2).Last().Trim(), ModifierCategory.Pseudo))
            {
                OptionText = pattern.OptionText,
                OptionId = pattern.OptionId,
            });
        }

        var ids = specialPatterns.Select(x => x.ApiId).Distinct().ToList();
        pseudoPatterns.RemoveAll(x => ids.Contains(x.ApiId));
        pseudoPatterns.AddRange(specialPatterns);
    }

    private Regex ComputePattern(string text, ModifierCategory? category = null, string? optionText = null)
    {
        text = RemoveSquareBrackets(text);
        if (optionText != null) optionText = RemoveSquareBrackets(optionText);

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
            ModifierCategory.Rune => "(?:\\ \\(rune\\))",
            ModifierCategory.Explicit => "(?:\\ \\((?:crafted|fractured)\\))?",
            _ => "",
        };

        var patternValue = Regex.Escape(text);
        patternValue = parenthesesPattern.Replace(patternValue, "(?:$1)?");
        patternValue = newLinePattern.Replace(patternValue, "\\n");

        if (string.IsNullOrEmpty(optionText))
        {
            patternValue = hashPattern.Replace(patternValue, "[-+0-9,.]+") + suffix;
        }
        else
        {
            var optionLines = new List<string>();
            foreach (var optionLine in newLinePattern.Split(optionText))
            {
                optionLines.Add(hashPattern.Replace(patternValue, Regex.Escape(optionLine)) + suffix);
            }

            patternValue = string.Join('\n', optionLines);
        }

        // For multiline modifiers, the category can be suffixed on all lines.
        if (!suffix.EndsWith("?")) suffix += "?";
        patternValue = patternValue.Replace("\\n", suffix + "\\n");

        return new Regex($"^{patternValue}$", RegexOptions.None);
    }

    private string ComputeFuzzyText(ModifierCategory category, string text, string? optionText = null)
    {
        text = RemoveSquareBrackets(text);
        if (optionText != null) optionText = RemoveSquareBrackets(optionText);

        var fuzzyValue = text;

        if (!string.IsNullOrEmpty(optionText))
        {
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
            ModifierCategory.Rune => " (rune)",
            _ => "",
        };

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
            var pattern = patternGroup.Value.FirstOrDefault(x => x.ApiId == id);
            if (pattern != null && pattern.Pattern.IsMatch(text))
            {
                return true;
            }
        }

        return false;
    }
}
