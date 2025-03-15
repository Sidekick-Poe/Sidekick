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
    private readonly Regex cleanOriginalTextPattern = new(" \\((?:implicit|enchant|crafted|veiled|fractured|scourge|crucible)\\)$");

    /// <inheritdoc/>
    public List<ModifierLine> Parse(ParsingItem parsingItem)
    {
        if (parsingItem.Header?.Category is Category.DivinationCard or Category.Gem)
        {
            return [];
        }

        return MatchModifiers(parsingItem)
            .Select(CreateModifierLine)
            // Trim modifier lines
            .Where(x => x.Modifiers.Count > 0)
            // Order the mods by the order they appear on the item.
            .OrderBy(x => x.BlockIndex)
            .ThenBy(x => x.LineIndex)
            .ToList();
    }

    private ModifierLine CreateModifierLine(ModifierMatch match)
    {
        var text = CreateString(match);
        var modifierLine = new ModifierLine(cleanOriginalTextPattern.Replace(text, string.Empty))
        {
            BlockIndex = match.Block.Index,
            LineIndex = match.Lines.First().Index,
        };

        var fuzzyLine = fuzzyService.CleanFuzzyText(text);
        var patterns = match.Patterns.OrderByDescending(x => Fuzz.Ratio(fuzzyLine, x.FuzzyText)).ToList();

        foreach (var pattern in patterns.Where(pattern => modifierLine.Modifiers.All(x => x.ApiId != pattern.ApiId)))
        {
            modifierLine.Modifiers.Add(new(text: pattern.ApiText)
            {
                ApiId = pattern.ApiId,
                Category = pattern.Category,
            });
        }

        if (modifierLine.Modifiers.All(x => x.Category == ModifierCategory.Pseudo))
        {
            // modifierLine.Text = modifierLine.Modifiers.FirstOrDefault()?.Text ?? modifierLine.Text;
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
        var allAvailablePatterns = GetAllAvailablePatterns(parsingItem);
        foreach (var block in parsingItem.Blocks.Where(x => !x.AnyParsed))
        {
            for (var lineIndex = 0; lineIndex < block.Lines.Count; lineIndex++)
            {
                var line = block.Lines[lineIndex];
                if (line.Parsed)
                {
                    continue;
                }

                var patterns = MatchModifierPatterns(block, line, lineIndex, allAvailablePatterns).ToList();
                if (patterns.Count == 0)
                {
                    // If we reach this point we have not found the modifier through traditional Regex means.
                    // Text from the game sometimes differ from the text from the API. We do a fuzzy search here to find the most common text.
                    patterns = [.. MatchModifierFuzzily(line, allAvailablePatterns)];
                }

                if (patterns.Count is 0)
                {
                    continue;
                }

                var maxLineCount = patterns.Max(x => x.LineCount);
                var lines = block.Lines.Skip(lineIndex).Take(maxLineCount).ToList();
                lines.ForEach(x => x.Parsed = true);

                lineIndex += maxLineCount - 1; // Increment the line index by one less of the pattern count. The default lineIndex++ will take care of the remaining increment.
                yield return new ModifierMatch(block, lines, patterns);
            }
        }
    }

    private static IEnumerable<ModifierDefinition> MatchModifierPatterns(ParsingBlock block, ParsingLine line, int lineIndex, IReadOnlyCollection<ModifierDefinition> allAvailablePatterns)
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

    private IEnumerable<ModifierDefinition> MatchModifierFuzzily(ParsingLine line, IReadOnlyCollection<ModifierDefinition> allAvailablePatterns)
    {
        if (line.Parsed)
        {
            yield break;
        }

        var fuzzyLine = fuzzyService.CleanFuzzyText(line.Text);

        var results = new List<(int Ratio, ModifierDefinition Pattern)>();
        var resultsLock = new object(); // Lock object to synchronize access to results

        Parallel.ForEach(allAvailablePatterns,
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

    private IReadOnlyCollection<ModifierDefinition> GetAllAvailablePatterns(ParsingItem parsingItem)
    {
        if (parsingItem.Header?.Category is Category.Sanctum)
        {
            return [ .. modifierProvider.Definitions[ModifierCategory.Sanctum]];
        }

        if (parsingItem.Header?.Category is Category.Map && parsingItem.Header.ItemCategory is "map.tablet")
        {
            return [ .. modifierProvider.Definitions[ModifierCategory.Implicit],
                     .. modifierProvider.Definitions[ModifierCategory.Explicit]];
        }

        return [ .. modifierProvider.Definitions.SelectMany(x => x.Value)];
    }

    private static void ParseModifierValue(ModifierLine modifierLine, ModifierDefinition? pattern)
    {
        switch (pattern)
        {
            case { IsOption: true }:
                modifierLine.OptionValue = pattern.OptionId;
                return;

            case { OptionId: int value }:
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
