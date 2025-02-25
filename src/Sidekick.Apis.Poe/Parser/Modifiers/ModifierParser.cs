using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using FuzzySharp;
using Sidekick.Apis.Poe.Fuzzy;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Parser.Modifiers;

public class ModifierParser
(
    IModifierProvider modifierProvider,
    IFuzzyService fuzzyService
) : IModifierParser
{
    private readonly Regex CleanOriginalTextPattern = new(" \\((?:implicit|enchant|crafted|veiled|fractured|scourge|crucible)\\)$");

    /// <inheritdoc/>
    public List<ModifierLine> Parse(ParsingItem parsingItem)
    {
        if (parsingItem.Header?.Category is Category.DivinationCard or Category.Gem)
        {
            return [];
        }

        return MatchModifiers(parsingItem)
            .Select(modifierMatch => CreateModifierLine(modifierMatch, parsingItem))
            // Trim modifier lines
            .Where(x => x.Modifiers.Count > 0)
            // Order the mods by the order they appear on the item.
            .OrderBy(x => parsingItem.Text.IndexOf(x.Text, StringComparison.InvariantCulture))
            .ToList();
    }

    private ModifierLine CreateModifierLine(ModifierMatch match, ParsingItem parsingItem)
    {
        var text = CreateString(match);
        var modifierLine = new ModifierLine(CleanOriginalTextPattern.Replace(text, string.Empty));

        var fuzzyLine = fuzzyService.CleanFuzzyText(text);
        var patterns = match.Patterns.OrderByDescending(x => Fuzz.Ratio(fuzzyLine, x.FuzzyText)).ToList();

        if (parsingItem.Header?.Category is Category.Sanctum)
        {
            patterns.RemoveAll(pattern => pattern.Category is not ModifierCategory.Sanctum);
        }

        if (parsingItem.Header?.Category is Category.Map && parsingItem.Header.ItemCategory is "map.tablet")
        {
            patterns.RemoveAll(pattern => pattern.Category is not (ModifierCategory.Implicit or ModifierCategory.Explicit));
        }

        foreach (var pattern in patterns)
        {
            if (!modifierLine.Modifiers.Any(x => x.Id == pattern.Id))
            {
                modifierLine.Modifiers.Add(new(text: pattern.Text)
                {
                    Id = pattern.Id,
                    Category = pattern.Category,
                });
            }
        }

        if (modifierLine.Modifiers.All(x => x.Category == ModifierCategory.Pseudo))
        {
            modifierLine.Text = modifierLine.Modifiers.FirstOrDefault()?.Text ?? modifierLine.Text;
        }

        ParseModifierValue(modifierLine, patterns.FirstOrDefault());
        return modifierLine;
    }

    private static string CreateString(ModifierMatch match)
    {
        var text = new StringBuilder();
        foreach (var line in match.Lines)
        {
            if (text.Length != 0)
            {
                text.Append('\n');
            }

            text.Append(line.Text);
            line.Parsed = true;
        }

        return text.ToString();
    }

    private IEnumerable<ModifierMatch> MatchModifiers(ParsingItem parsingItem)
    {
        foreach (var block in parsingItem.Blocks.Where(x => !x.AnyParsed))
        {
            for (var lineIndex = 0; lineIndex < block.Lines.Count; lineIndex++)
            {
                var line = block.Lines[lineIndex];
                if (line.Parsed)
                {
                    continue;
                }

                var patterns = MatchModifierPatterns(block, line, lineIndex).ToList();
                if (patterns.Count == 0)
                {
                    // If we reach this point we have not found the modifier through traditional Regex means.
                    // Text from the game sometimes differ from the text from the API. We do a fuzzy search here to find the most common text.
                    patterns = [.. MatchModifierFuzzily(line)];
                }

                if (patterns.Count is 0)
                {
                    continue;
                }

                var maxLineCount = patterns.Max(x => x.LineCount);
                lineIndex += maxLineCount - 1; // Increment the line index by one less of the pattern count. The default lineIndex++ will take care of the remaining increment.
                yield return new ModifierMatch(block, block.Lines.Skip(lineIndex).Take(maxLineCount), patterns);
            }
        }
    }

    private IEnumerable<ModifierPattern> MatchModifierPatterns(ParsingBlock block, ParsingLine line, int lineIndex)
    {
        foreach (var pattern in modifierProvider.Patterns.SelectMany(x => x.Value))
        {
            var isMultilineModifierMatch = pattern.LineCount > 1 && pattern.Pattern.IsMatch(string.Join('\n', block.Lines.Skip(lineIndex).Take(pattern.LineCount)));
            var isSingleModifierMatch = pattern.Pattern.IsMatch(line.Text);

            if (isMultilineModifierMatch || isSingleModifierMatch)
            {
                yield return pattern;
            }
        }
    }

    private IEnumerable<ModifierPattern> MatchModifierFuzzily(ParsingLine line)
    {
        if (line.Parsed)
        {
            yield break;
        }

        var fuzzyLine = fuzzyService.CleanFuzzyText(line.Text);

        var results = new List<(int Ratio, ModifierPattern Pattern)>();
        var resultsLock = new object(); // Lock object to synchronize access to results

        Parallel.ForEach(modifierProvider.Patterns.SelectMany(x => x.Value),
                         (x) =>
                         {
                             var ratio = Fuzz.Ratio(fuzzyLine, x.FuzzyText, FuzzySharp.PreProcess.PreprocessMode.None);
                             if (ratio <= 75)
                             {
                                 return;
                             }

                             lock (resultsLock) // Lock before accessing the shared list
                             {
                                 results.Add((ratio, x));
                             }
                         });

        foreach (var (ratio, pattern) in results.OrderByDescending(x => x.Ratio))
        {
            yield return pattern;
        }
    }

    private static void ParseModifierValue(ModifierLine modifierLine, ModifierPattern? pattern)
    {
        switch (pattern)
        {
            case { IsOption: true }:
                modifierLine.OptionValue = pattern.Value;
                return;

            case { Value: int value }:
                modifierLine.Values.Add(value);
                return;
            
            case null: 
                return;
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
}
