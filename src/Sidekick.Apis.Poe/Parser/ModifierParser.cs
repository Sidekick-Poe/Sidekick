using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using FuzzySharp;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Parser
{
    public class ModifierParser(IModifierProvider modifierProvider) : IModifierParser
    {
        private readonly Regex CleanFuzzyPattern = new("[-+0-9%#]");
        private readonly Regex TrimPattern = new(@"\s+");
        private readonly Regex CleanOriginalTextPattern = new(" \\((?:implicit|enchant|crafted|veiled|fractured|scourge|crucible)\\)$");

        private string CleanFuzzyText(string text)
        {
            text = CleanFuzzyPattern.Replace(text, string.Empty);
            return TrimPattern.Replace(text, " ").Trim();
        }

        /// <inheritdoc/>
        public List<ModifierLine> Parse(ParsingItem parsingItem)
        {
            var modifierLines = new List<ModifierLine>();

            foreach (var match in MatchModifiers(parsingItem))
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

                var modifierLine = new ModifierLine(text: CleanOriginalTextPattern.Replace(text.ToString(), string.Empty));
                modifierLines.Add(modifierLine);

                var fuzzyLine = CleanFuzzyText(text.ToString());
                var patterns = match.Patterns.OrderByDescending(x => Fuzz.Ratio(fuzzyLine, x.FuzzyText)).ToList();
                foreach (var pattern in patterns)
                {
                    if (modifierLine.Modifiers.Any(x => x.Id == pattern.Id))
                    {
                        continue;
                    }

                    var modifier = new Modifier(text: pattern.Text)
                    {
                        Id = pattern.Id,
                        Category = pattern.Category,
                    };
                    modifierLine.Modifiers.Add(modifier);
                }

                if (modifierLine.Modifiers.All(x => x.Category == ModifierCategory.Pseudo))
                {
                    modifierLine.Text = modifierLine.Modifiers.FirstOrDefault()?.Text ?? modifierLine.Text;
                }

                ParseModifierValues(modifierLine, patterns);
            }

            // Order the mods by the order they appear on the item.
            modifierLines = modifierLines.OrderBy(x => parsingItem.Text.IndexOf(x.Text, StringComparison.InvariantCulture)).ToList();

            // Trim modifier lines
            modifierLines.RemoveAll(x => x.Modifiers.Count == 0);

            return modifierLines;
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
                    if (!patterns.Any())
                    {
                        // If we reach this point we have not found the modifier through traditional Regex means.
                        // Text from the game sometimes differ from the text from the API. We do a fuzzy search here to find the most common text.
                        patterns = MatchModifierFuzzily(parsingItem, line).ToList();
                    }

                    if (!patterns.Any())
                    {
                        continue;
                    }

                    var maxLineCount = patterns.Max(x => x.LineCount);
                    lineIndex += maxLineCount - 1; // Increment the line index by one less of the pattern count. The default lineIndex++ will take care of the remaining increment.
                    yield return new ModifierMatch(block, block.Lines.Skip(lineIndex).Take(maxLineCount), patterns.ToList());
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

        private IEnumerable<ModifierPattern> MatchModifierFuzzily(ParsingItem item, ParsingLine line)
        {
            if (line.Parsed)
            {
                yield break;
            }

            var fuzzyLine = CleanFuzzyText(line.Text);
            var fuzzies = new List<FuzzyResult>();

            Parallel.ForEach(modifierProvider.FuzzyDictionary, (x) =>
            {
                var ratio = Fuzz.Ratio(fuzzyLine, x.Key, FuzzySharp.PreProcess.PreprocessMode.None);
                if (ratio > 75)
                {
                    fuzzies.Add(new FuzzyResult(
                        ratio: ratio,
                        patterns: x.Value
                    ));
                }
            });

            foreach (var fuzzy in fuzzies.OrderByDescending(x => x.Ratio))
            {
                foreach (var modifier in fuzzy.Patterns)
                {
                    yield return modifier;
                }
            }
        }

        private void ParseModifierValues(ModifierLine modifierLine, IEnumerable<ModifierPattern> patterns)
        {
            var pattern = patterns.FirstOrDefault();
            if (pattern == null)
            {
                return;
            }

            if (pattern.IsOption)
            {
                modifierLine.OptionValue = pattern.Value;
                return;
            }

            if (pattern.Value.HasValue)
            {
                modifierLine.Values.Add(pattern.Value.Value);
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
}
