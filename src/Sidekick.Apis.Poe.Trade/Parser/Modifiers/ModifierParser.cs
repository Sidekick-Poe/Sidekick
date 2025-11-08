using System.Globalization;
using System.Text.RegularExpressions;
using FuzzySharp;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Fuzzy;
using Sidekick.Apis.Poe.Trade.Modifiers;
using Sidekick.Apis.Poe.Trade.Modifiers.Models;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Modifiers;

public class ModifierParser
(
    IModifierProvider modifierProvider,
    IFuzzyService fuzzyService,
    ISettingsService settingsService
) : IModifierParser
{
    /// <inheritdoc/>
    public void Parse(Item item)
    {
        if (item.ApiInformation.Category is Category.DivinationCard or Category.Gem) return;

        var modifiers = MatchModifiers(item)
            // Trim modifier lines
            .Where(x => x.ApiInformation.Count > 0)

            // Order the mods by the order they appear on the item.
            .OrderBy(x => x.BlockIndex)
            .ThenBy(x => x.LineIndex)
            .ToList();

        item.Modifiers.Clear();
        item.Modifiers.AddRange(modifiers);
    }

    private IEnumerable<Modifier> MatchModifiers(Item item)
    {
        var allAvailablePatterns = GetAllAvailablePatterns(item);
        foreach (var block in item.Text.Blocks.Where(x => !x.AnyParsed))
        {
            for (var lineIndex = 0; lineIndex < block.Lines.Count; lineIndex++)
            {
                if (block.Lines[lineIndex].Parsed) continue;

                var definitions = MatchModifierPatterns(block, lineIndex, allAvailablePatterns).ToList();
                var matchFuzzily = definitions.Count == 0;
                if (matchFuzzily)
                {
                    // If we reach this point we have not found the modifier through traditional Regex means.
                    // Text from the game sometimes differ from the text from the API. We do a fuzzy search here to find the most common text.
                    definitions = MatchModifierFuzzily(block, lineIndex, allAvailablePatterns).ToList();
                }

                if (definitions.Count is 0) continue;

                var maxLineCount = definitions.Max(x => x.LineCount);
                var lines = block.Lines.Skip(lineIndex).Take(maxLineCount).ToList();
                lines.ForEach(x => x.Parsed = true);

                yield return CreateModifier(block, lines, definitions, matchFuzzily);
            }
        }
    }

    private static IEnumerable<ModifierDefinition> MatchModifierPatterns(TextBlock block, int lineIndex, IReadOnlyCollection<ModifierDefinition> allAvailablePatterns)
    {
        foreach (var pattern in allAvailablePatterns)
        {
            // Multiple line modifiers
            if (pattern.LineCount > 1 && pattern.Pattern.IsMatch(string.Join('\n', block.Lines.Skip(lineIndex).Take(pattern.LineCount))))
            {
                yield return pattern;
                continue;
            }

            // Single line modifiers
            if (pattern.Pattern.IsMatch(block.Lines[lineIndex].Text))
            {
                yield return pattern;
            }
        }
    }

    private IEnumerable<ModifierDefinition> MatchModifierFuzzily(TextBlock block, int lineIndex, IReadOnlyCollection<ModifierDefinition> allAvailablePatterns)
    {
        var category = block.Lines[lineIndex].Text.ParseCategory();
        if (category == ModifierCategory.Mutated) category = ModifierCategory.Explicit;

        var cleanLine = block.Lines[lineIndex].Text.RemoveCategory();
        var fuzzySingleLine = fuzzyService.CleanFuzzyText(cleanLine);
        string? fuzzyDoubleLine = null;
        if (lineIndex < block.Lines.Count - 1)
        {
            fuzzyDoubleLine = fuzzyService.CleanFuzzyText(cleanLine + " " + block.Lines[lineIndex + 1].Text);
        }

        var results = new List<(int Ratio, ModifierDefinition Pattern)>();
        var resultsLock = new object();// Lock object to synchronize access to results

        Parallel.ForEach(allAvailablePatterns,
                         definition =>
                         {
                             if (category != ModifierCategory.Undefined && definition.Category != category) return;

                             if (category == ModifierCategory.Undefined && !ModifierCategoryExtensions.ExplicitCategories.Contains(definition.Category)) return;

                             var compareLine = definition.LineCount switch
                             {
                                 2 => fuzzyDoubleLine ?? fuzzySingleLine,
                                 _ => fuzzySingleLine,
                             };

                             var ratio = Fuzz.Ratio(compareLine, definition.FuzzyText, FuzzySharp.PreProcess.PreprocessMode.None);
                             if (ratio <= 75)
                             {
                                 return;
                             }

                             lock (resultsLock)// Lock before accessing the shared list
                             {
                                 results.Add((ratio, definition));
                             }
                         });

        foreach (var (_, pattern) in results.OrderByDescending(x => x.Ratio))
        {
            yield return pattern;
        }
    }

    private IReadOnlyCollection<ModifierDefinition> GetAllAvailablePatterns(Item item)
    {
        return item.ApiInformation.Category switch
        {
            Category.Sanctum =>
            [
                .. modifierProvider.Definitions[ModifierCategory.Sanctum]
            ],
            Category.Map when item.Properties.ItemClass == ItemClass.Tablet =>
            [
                .. modifierProvider.Definitions[ModifierCategory.Implicit],
                .. modifierProvider.Definitions[ModifierCategory.Explicit]
            ],
            _ =>
            [
                .. modifierProvider.Definitions.SelectMany(x => x.Value)
            ]
        };

    }

    private Modifier CreateModifier(TextBlock block, List<TextLine> lines, List<ModifierDefinition> definitions, bool matchedFuzzily)
    {
        var text = string.Join('\n', lines.Select(x => x.Text));
        var category = text.ParseCategory();

        var modifier = new Modifier(text.RemoveCategory())
        {
            BlockIndex = block.Index,
            LineIndex = lines.First().Index,
            MatchedFuzzily = matchedFuzzily,
        };

        var fuzzyLine = fuzzyService.CleanFuzzyText(text);
        var filteredDefinitions = definitions
            .DistinctBy(x => x.ApiId)
            .OrderByDescending(x => Fuzz.Ratio(fuzzyLine, x.FuzzyText))
            .ToList();

        if (filteredDefinitions.Any(x => x.Category == ModifierCategory.Pseudo)
            && filteredDefinitions.Any(x => x.Category != ModifierCategory.Pseudo))
        {
            filteredDefinitions = filteredDefinitions.Where(x => x.Category != ModifierCategory.Pseudo).ToList();
        }

        foreach (var definition in filteredDefinitions)
        {
            modifier.ApiInformation.Add(new(apiText: definition.ApiText)
            {
                ApiId = definition.ApiId,
                Category = category switch
                {
                    ModifierCategory.Mutated => ModifierCategory.Mutated,
                    _ => definition.Category,
                },
            });

            foreach (var secondaryDefinition in definition.SecondaryDefinitions)
            {
                modifier.ApiInformation.Add(new(apiText: secondaryDefinition.ApiText)
                {
                    ApiId = secondaryDefinition.ApiId,
                    Category = category switch
                    {
                        ModifierCategory.Mutated => ModifierCategory.Mutated,
                        _ => secondaryDefinition.Category,
                    },
                });
            }
        }

        ParseModifierValue(modifier, filteredDefinitions.FirstOrDefault());
        return modifier;
    }

    private static void ParseModifierValue(Modifier modifier, ModifierDefinition? pattern)
    {
        switch (pattern)
        {
            case
            {
                IsOption: true
            }:
                modifier.OptionValue = pattern.OptionId;
                return;

            case
            {
                OptionId: int value,
            }:
                modifier.Values.Add(value);
                return;

            case null: return;
        }

        var matches = new Regex("([-+0-9,.]+)").Matches(modifier.Text.Split('\n').First());
        foreach (Match match in matches)
        {
            if (double.TryParse(match.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue))
            {
                modifier.Values.Add(parsedValue);
            }
        }
    }

    public async Task<List<ModifierFilter>> GetFilters(Item item)
    {
        // No filters for divination cards, etc.
        if (item.ApiInformation.Category is Category.DivinationCard
            or Category.Gem
            or Category.ItemisedMonster
            or Category.Leaguestone
            or Category.Unknown)
        {
            return [];
        }

        var enableAllFilters = await settingsService.GetBool(SettingKeys.PriceCheckEnableAllFilters);
        var enableFiltersByRegexSetting = await settingsService.GetString(SettingKeys.PriceCheckEnableFiltersByRegex);
        Regex? enableFiltersByRegex = null;
        if (!string.IsNullOrWhiteSpace(enableFiltersByRegexSetting))
        {
            enableFiltersByRegex = new Regex(enableFiltersByRegexSetting, RegexOptions.IgnoreCase);
        }

        var result = new List<ModifierFilter>();
        foreach (var modifier in item.Modifiers)
        {
            var filter = new ModifierFilter(modifier)
            {
                Checked = enableAllFilters || (enableFiltersByRegex?.IsMatch(modifier.Text) ?? false),
            };
            result.Add(filter);
        }

        return result;
    }

}
