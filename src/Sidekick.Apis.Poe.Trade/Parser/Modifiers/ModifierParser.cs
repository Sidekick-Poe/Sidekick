using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using FuzzySharp;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Fuzzy;
using Sidekick.Apis.Poe.Trade.Modifiers;
using Sidekick.Apis.Poe.Trade.Modifiers.Models;
using Sidekick.Apis.Poe.Trade.Parser.Modifiers.Filters;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Modifiers;

public class ModifierParser
(
    IModifierProvider modifierProvider,
    IFuzzyService fuzzyService,
    ISettingsService settingsService
) : IModifierParser
{
    private readonly Regex parseCategoryPattern = new(@" \((implicit|enchant|crafted|veiled|fractured|scourge|crucible|rune|desecrated)\)");

    /// <inheritdoc/>
    public void Parse(Item item)
    {
        if (item.Header.Category is Category.DivinationCard or Category.Gem) return;

        var modifiers = MatchModifiers(item)
            .Select(CreateModifierLine)

            // Trim modifier lines
            .Where(x => x.Modifiers.Count > 0)

            // Order the mods by the order they appear on the item.
            .OrderBy(x => x.BlockIndex)
            .ThenBy(x => x.LineIndex)
            .ToList();

        item.ModifierLines.Clear();
        item.ModifierLines.AddRange(modifiers);
    }

    private IEnumerable<ModifierMatch> MatchModifiers(Item item)
    {
        var allAvailablePatterns = GetAllAvailablePatterns(item.Header);
        foreach (var block in item.Text.Blocks.Where(x => !x.AnyParsed))
        {
            for (var lineIndex = 0; lineIndex < block.Lines.Count; lineIndex++)
            {
                var line = block.Lines[lineIndex];
                if (line.Parsed) continue;

                var definitions = MatchModifierPatterns(block, line, lineIndex, allAvailablePatterns).ToList();
                if (definitions.Count == 0)
                {
                    // If we reach this point we have not found the modifier through traditional Regex means.
                    // Text from the game sometimes differ from the text from the API. We do a fuzzy search here to find the most common text.
                    definitions = [.. MatchModifierFuzzily(block, line, allAvailablePatterns)];
                }

                if (definitions.Count is 0) continue;

                var maxLineCount = definitions.Max(x => x.LineCount);
                var lines = block.Lines.Skip(lineIndex).Take(maxLineCount).ToList();
                lines.ForEach(x => x.Parsed = true);

                yield return new ModifierMatch(block, lines, definitions);
            }
        }
    }

    private static IEnumerable<ModifierDefinition> MatchModifierPatterns(TextBlock block, TextLine line, int lineIndex, IReadOnlyCollection<ModifierDefinition> allAvailablePatterns)
    {
        foreach (var pattern in allAvailablePatterns)
        {
            var isMultilineModifierMatch = pattern.LineCount > 1 && pattern.Pattern.IsMatch(string.Join('\n', block.Lines.Skip(lineIndex).Take(pattern.LineCount)));
            var isSingleModifierMatch = pattern.Pattern.IsMatch(line.Text);

            if (isMultilineModifierMatch || isSingleModifierMatch)
            {
                yield return pattern;
            }
        }
    }

    private IEnumerable<ModifierDefinition> MatchModifierFuzzily(TextBlock block, TextLine line, IReadOnlyCollection<ModifierDefinition> allAvailablePatterns)
    {
        if (line.Parsed) yield break;

        var cleanLine = parseCategoryPattern.Replace(line.Text, string.Empty);
        var fuzzySingleLine = fuzzyService.CleanFuzzyText(cleanLine);
        string? fuzzyDoubleLine = null;
        var lineIndex = block.Lines.IndexOf(line);
        if (lineIndex < block.Lines.Count - 1)
        {
            fuzzyDoubleLine = fuzzyService.CleanFuzzyText(cleanLine + " " + block.Lines[lineIndex + 1].Text);
        }

        var results = new List<(int Ratio, ModifierDefinition Pattern)>();
        var resultsLock = new object();// Lock object to synchronize access to results

        Parallel.ForEach(allAvailablePatterns,
                         definition =>
                         {
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

    private IReadOnlyCollection<ModifierDefinition> GetAllAvailablePatterns(ItemHeader header)
    {
        return header.Category switch
        {
            Category.Sanctum =>
            [
                .. modifierProvider.Definitions[ModifierCategory.Sanctum]
            ],
            Category.Map when header.ItemClass == ItemClass.Tablet =>
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

    private ModifierLine CreateModifierLine(ModifierMatch match)
    {
        var textBuilder = new StringBuilder();
        foreach (var line in match.Lines)
        {
            if (textBuilder.Length != 0) textBuilder.Append('\n');

            textBuilder.Append(line.Text);
            line.Parsed = true;
        }

        var text = textBuilder.ToString();
        var textCategoryMatches = parseCategoryPattern.Matches(text);
        var category = textCategoryMatches.Count > 0 ? textCategoryMatches[0].Groups[1].Value : string.Empty;

        var modifierLine = new ModifierLine(parseCategoryPattern.Replace(text, string.Empty))
        {
            BlockIndex = match.Block.Index,
            LineIndex = match.Lines.First().Index,
        };

        var fuzzyLine = fuzzyService.CleanFuzzyText(text);
        var patterns = match.Definitions.OrderByDescending(x =>
            {
                if (!string.IsNullOrEmpty(category) && category == x.Category.GetValueAttribute())
                {
                    return 1;
                }

                return 0;
            })
            .ThenByDescending(x => Fuzz.Ratio(fuzzyLine, x.FuzzyText))
            .ToList();

        foreach (var pattern in patterns.Where(pattern => modifierLine.Modifiers.All(x => x.ApiId != pattern.ApiId)))
        {
            modifierLine.Modifiers.Add(new(text: pattern.ApiText)
            {
                ApiId = pattern.ApiId,
                Category = pattern.Category,
            });
        }

        ParseModifierValue(modifierLine, patterns.FirstOrDefault());
        return modifierLine;
    }

    private static void ParseModifierValue(ModifierLine modifierLine, ModifierDefinition? pattern)
    {
        switch (pattern)
        {
            case
            {
                IsOption: true
            }:
                modifierLine.OptionValue = pattern.OptionId;
                return;

            case
            {
                OptionId: int value
            }:
                modifierLine.Values.Add(value);
                return;

            case null: return;
        }

        var matches = new Regex("([-+0-9,.]+)").Matches(modifierLine.Text);
        foreach (Match match in matches)
        {
            if (double.TryParse(match.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue))
            {
                modifierLine.Values.Add(parsedValue);
            }
        }
    }

    public async Task<List<ModifierFilter>> GetFilters(Item item)
    {
        // No filters for divination cards, etc.
        if (item.Header.Category is Category.DivinationCard
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
        foreach (var modifierLine in item.ModifierLines)
        {
            var modifier = new ModifierFilter(modifierLine)
            {
                Checked = enableAllFilters || (enableFiltersByRegex?.IsMatch(modifierLine.Text) ?? false),
            };
            result.Add(modifier);
        }

        return result;
    }

}
