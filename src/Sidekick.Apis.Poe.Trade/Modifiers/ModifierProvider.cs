using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Fuzzy;
using Sidekick.Apis.Poe.Trade.Modifiers.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Modifiers;

public class ModifierProvider
(
    ICacheProvider cacheProvider,
    ITradeApiClient tradeApiClient,
    IInvariantModifierProvider invariantModifierProvider,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService,
    IFuzzyService fuzzyService
) : IModifierProvider
{
    private readonly Regex parseHashPattern = new("\\#");

    private readonly Regex newLinePattern = new(@"(?:\\)*[\r\n]+");
    private readonly Regex hashPattern = new(@"\\#");
    private readonly Regex parenthesesPattern = new(@"((?:\\\ )*\\\([^\(\)]*\\\))");

    private record ModifierReplaceEntry(Regex Pattern, string Replacement);

    private List<ModifierReplaceEntry> ReplacementPatterns { get; } = [];

    public Dictionary<ModifierCategory, List<ModifierDefinition>> Definitions { get; } = new();

    /// <inheritdoc/>
    public int Priority => 200;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        var cacheKey = $"{game.GetValueAttribute()}_Modifiers";
        var apiCategories = await cacheProvider.GetOrSet(cacheKey, () => tradeApiClient.FetchData<ApiCategory>(game, gameLanguageProvider.Language, "stats"), (cache) => cache.Result.Any());
        if (apiCategories == null) throw new SidekickException("Could not fetch modifiers from the trade API.");

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

        Parallel.ForEach(apiCategories.Result, apiCategory =>
        {
            var modifierCategory = GetModifierCategory(apiCategory.Entries[0].Id);
            var patterns = ComputeCategoryPatterns(apiCategory, modifierCategory);

            if (!Definitions.TryAdd(modifierCategory, patterns))
            {
                Definitions[modifierCategory].AddRange(patterns);
            }
        });

        ComputeSecondaryDefinitions();

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
        if (apiCategory.Entries.Count == 0 || modifierCategory == ModifierCategory.Undefined) return [];

        var patterns = new List<ModifierDefinition>();
        foreach (var entry in apiCategory.Entries)
        {
            patterns.AddRange(ComputeDefinition(modifierCategory, entry));
        }

        return patterns;
    }
    private IEnumerable<ModifierDefinition> ComputeDefinition(ModifierCategory modifierCategory, ApiModifier entry)
    {
        if (invariantModifierProvider.IgnoreModifierIds.Contains(entry.Id)) yield break;

        entry.Text = entry.Text.RemoveSquareBrackets();

        if (entry.Option?.Options.Count > 0)
        {
            foreach (var option in entry.Option.Options)
            {
                if (option.Text == null) continue;
                option.Text = option.Text.RemoveSquareBrackets();
                yield return new ModifierDefinition(modifierCategory, entry.Id, apiText: ComputeOptionText(entry.Text, option.Text), fuzzyText: ComputeFuzzyText(entry.Text, option.Text), pattern: ComputePattern(entry.Text, modifierCategory, option.Text))
                {
                    OptionId = option.Id,
                    OptionText = option.Text,
                };
            }
        }
        else
        {
            yield return new ModifierDefinition(modifierCategory, entry.Id, entry.Text, fuzzyText: ComputeFuzzyText(entry.Text), pattern: ComputePattern(entry.Text, modifierCategory));
        }
    }

    /// <inheritdoc/>
    public ModifierCategory GetModifierCategory(string? apiId)
    {
        var value = apiId?.Split('.').First();
        return value.GetEnumFromValue<ModifierCategory>();
    }

    private void FillSpecialPseudoPattern(List<ModifierDefinition> pseudoPatterns, List<ModifierDefinition> patterns)
    {
        var specialPatterns = new List<ModifierDefinition>();
        foreach (var group in patterns.GroupBy(x => x.ApiId))
        {
            var pattern = group.OrderBy(x => x.OptionId).First();
            specialPatterns.Add(new ModifierDefinition(pattern.Category, pattern.ApiId, pattern.ApiText, fuzzyText: ComputeFuzzyText(pattern.ApiText), pattern: ComputePattern(pattern.ApiText.Split(':', 2).Last().Trim(), ModifierCategory.Pseudo))
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
        text = text.RemoveSquareBrackets();
        if (optionText != null) optionText = optionText.RemoveSquareBrackets();

        // The notes in parentheses are never translated by the game.
        // We should be fine hardcoding them this way.
        var suffix = category switch
        {
            ModifierCategory.Enchant => @"\ \(enchant\)",
            ModifierCategory.Rune => @"\ \(rune\)",
            ModifierCategory.Implicit => @"\ \(implicit\)",
            ModifierCategory.Veiled => @"\ \(veiled\)",
            ModifierCategory.Scourge => @"\ \(scourge\)",
            ModifierCategory.Crucible => @"\ \(crucible\)",
            ModifierCategory.Crafted => @"\ \(crafted\)",
            ModifierCategory.Fractured => @"\ \(fractured\)",
            ModifierCategory.Desecrated => @"\ \(desecrated\)",
            ModifierCategory.Explicit => @"(?:\ \(mutated\))?",
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

        // For multiline modifiers, the category can be suffixed on all lines.
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
            var pattern = patternGroup.Value.FirstOrDefault(x => x.ApiId == id);
            if (pattern != null && pattern.Pattern.IsMatch(text))
            {
                return true;
            }
        }

        return false;
    }

    private void ComputeSecondaryDefinitions()
    {
        var explicitDefinitions = Definitions.GetValueOrDefault(ModifierCategory.Explicit);
        if (explicitDefinitions == null) return;

        foreach (var group in Definitions)
        {
            if (!group.Key.HasExplicitModifier()) continue;

            foreach (var definition in group.Value)
            {
                definition.SecondaryDefinitions.AddRange(explicitDefinitions.Where(x => x.ApiText == definition.ApiText));

            }
        }
    }

}
