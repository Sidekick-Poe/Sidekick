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
    private readonly Regex cleanFuzzyPattern = new("[-+0-9%#]");
    private readonly Regex trimPattern = new(@"\s+");

    public Dictionary<ModifierCategory, List<ModifierPattern>> Patterns { get; } = new();

    public Dictionary<string, List<ModifierPattern>> FuzzyDictionary { get; private set; } = new();

    /// <inheritdoc/>
    public int Priority => 200;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();
        var cacheKey = $"{game.GetValueAttribute()}_Modifiers";
        var apiCategories = await cacheProvider.GetOrSet(cacheKey, () => poeTradeClient.Fetch<ApiCategory>(game, gameLanguageProvider.Language, "data/stats"), (cache) => cache.Result.Any());

        foreach (var apiCategory in apiCategories.Result)
        {
            var modifierCategory = GetModifierCategory(apiCategory.Entries[0].Id);
            var patterns = ComputeCategoryPatterns(apiCategory, modifierCategory);

            if (!Patterns.TryAdd(modifierCategory, patterns))
            {
                Patterns[modifierCategory].AddRange(patterns);
            }
        }

        FuzzyDictionary = ComputeFuzzyDictionary(Patterns.SelectMany(x => x.Value));

        // Prepare special pseudo patterns
        var pseudoPatterns = Patterns.GetValueOrDefault(ModifierCategory.Pseudo) ?? [];

        var incursionPatterns = pseudoPatterns.Where(x => invariantModifierProvider.IncursionRoomModifierIds.Contains(x.Id)).ToList();
        ComputeSpecialPseudoPattern(pseudoPatterns, incursionPatterns);

        var logbookPatterns = pseudoPatterns.Where(x => invariantModifierProvider.LogbookFactionModifierIds.Contains(x.Id)).ToList();
        ComputeSpecialPseudoPattern(pseudoPatterns, logbookPatterns);
    }

    private List<ModifierPattern> ComputeCategoryPatterns(ApiCategory apiCategory, ModifierCategory modifierCategory)
    {
        if (apiCategory.Entries.Count == 0 || modifierCategory == ModifierCategory.Undefined)
        {
            return [];
        }

        var patterns = new List<ModifierPattern>();
        foreach (var entry in apiCategory.Entries)
        {
            entry.Text = RemoveSquareBrackets(entry.Text);

            var options = entry.Option?.Options ?? [];
            if (options.Count > 0)
            {
                foreach (var option in options)
                {
                    var optionText = option.Text;
                    if (optionText == null)
                    {
                        continue;
                    }

                    optionText = RemoveSquareBrackets(optionText);
                    patterns.Add(new ModifierPattern(modifierCategory, entry.Id, options.Any(), text: ComputeOptionText(entry.Text, optionText), fuzzyText: ComputeFuzzyText(modifierCategory, entry.Text, optionText), pattern: ComputePattern(entry.Text, modifierCategory, optionText))
                    {
                        Value = option.Id,
                    });
                }
            }
            else
            {
                patterns.Add(new ModifierPattern(modifierCategory, entry.Id, options.Any(), entry.Text, fuzzyText: ComputeFuzzyText(modifierCategory, entry.Text), pattern: ComputePattern(entry.Text, modifierCategory)));
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

        return CleanFuzzyText(fuzzyValue);
    }

    private string CleanFuzzyText(string text)
    {
        text = cleanFuzzyPattern.Replace(text, string.Empty);
        return trimPattern.Replace(text, " ").Trim();
    }

    private Dictionary<string, List<ModifierPattern>> ComputeFuzzyDictionary(IEnumerable<ModifierPattern> patterns)
    {
        var result = new Dictionary<string, List<ModifierPattern>>();

        foreach (var pattern in patterns)
        {
            if (!result.ContainsKey(pattern.FuzzyText))
            {
                result.Add(pattern.FuzzyText, new());
            }

            result[pattern.FuzzyText].Add(pattern);
        }

        return result;
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
