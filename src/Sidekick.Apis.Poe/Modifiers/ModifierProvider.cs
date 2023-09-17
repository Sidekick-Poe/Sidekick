using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using FuzzySharp;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Apis.Poe.Parser;
using Sidekick.Common.Cache;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Modifiers
{
    public class ModifierProvider : IModifierProvider
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IPoeTradeClient poeTradeClient;
        private readonly IEnglishModifierProvider englishModifierProvider;
        private readonly Regex ParseHashPattern = new("\\#");
        private readonly Regex NewLinePattern = new("(?:\\\\)*[\\r\\n]+");
        private readonly Regex HashPattern = new("\\\\#");
        private readonly Regex ParenthesesPattern = new("((?:\\\\\\ )*\\\\\\([^\\(\\)]*\\\\\\))");
        private readonly Regex CleanFuzzyPattern = new("[-+0-9%#]");
        private readonly Regex TrimPattern = new(@"\s+");
        private readonly Regex CleanOriginalTextPattern = new(" \\((?:implicit|enchant|crafted|veiled|fractured|scourge|crucible)\\)$");

        public ModifierProvider(
            ICacheProvider cacheProvider,
            IPoeTradeClient poeTradeClient,
            IEnglishModifierProvider englishModifierProvider)
        {
            this.cacheProvider = cacheProvider;
            this.poeTradeClient = poeTradeClient;
            this.englishModifierProvider = englishModifierProvider;
        }

        private Dictionary<ModifierCategory, List<ModifierPattern>> Patterns { get; } = new();
        private Dictionary<string, List<ModifierPattern>> FuzzyDictionary { get; } = new();

        /// <inheritdoc/>
        public InitializationPriority Priority => InitializationPriority.Low;

        /// <inheritdoc/>
        public async Task Initialize()
        {
            var result = await cacheProvider.GetOrSet(
                "ModifierProvider",
                () => poeTradeClient.Fetch<ApiCategory>("data/stats"));
            var categories = result.Result;

            foreach (var category in categories)
            {
                if (!category.Entries.Any())
                {
                    continue;
                }

                var modifierCategory = GetModifierCategory(category.Entries[0].Id);
                if (modifierCategory == ModifierCategory.Undefined)
                {
                    continue;
                }

                var patterns = new List<ModifierPattern>();
                foreach (var entry in category.Entries)
                {
                    if (entry.Text == null || entry.Id == null)
                    {
                        continue;
                    }

                    var isOption = entry.Option?.Options?.Any() ?? false;
                    if (isOption)
                    {
                        for (var i = 0; i < entry.Option?.Options.Count; i++)
                        {
                            var optionText = entry.Option.Options[i].Text;
                            if (optionText == null)
                            {
                                continue;
                            }

                            patterns.Add(new ModifierPattern(
                                category: modifierCategory,
                                id: entry.Id,
                                isOption: entry.Option?.Options?.Any() ?? false,
                                text: ComputeOptionText(entry.Text, optionText),
                                fuzzyText: ComputeFuzzyText(modifierCategory, entry.Text, optionText),
                                pattern: ComputePattern(entry.Text, modifierCategory, optionText))
                            {
                                Value = entry.Option.Options[i].Id,
                            });
                        }
                    }
                    else
                    {
                        patterns.Add(new ModifierPattern(
                            category: modifierCategory,
                            id: entry.Id,
                            isOption: entry.Option?.Options?.Any() ?? false,
                            text: entry.Text,
                            fuzzyText: ComputeFuzzyText(modifierCategory, entry.Text),
                            pattern: ComputePattern(entry.Text, modifierCategory)));
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
            var incursionPatterns = Patterns[ModifierCategory.Pseudo]
                .Where(x => englishModifierProvider.IncursionRooms.Contains(x.Id))
                .ToList();
            ComputeSpecialPseudoPattern(incursionPatterns);

            var logbookPatterns = Patterns[ModifierCategory.Pseudo]
                .Where(x => englishModifierProvider.LogbookFactions.Contains(x.Id))
                .ToList();
            ComputeSpecialPseudoPattern(logbookPatterns);
        }

        /// <inheritdoc/>
        public ModifierCategory GetModifierCategory(string? apiId)
        {
            return apiId?.Split('.').First() switch
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
        }

        private void ComputeSpecialPseudoPattern(List<ModifierPattern> patterns)
        {
            var specialPatterns = new List<ModifierPattern>();
            foreach (var group in patterns.GroupBy(x => x.Id))
            {
                var pattern = group.OrderBy(x => x.Value).First();
                specialPatterns.Add(new ModifierPattern(
                    category: pattern.Category,
                    id: pattern.Id,
                    isOption: pattern.IsOption,
                    text: pattern.Text,
                    fuzzyText: ComputeFuzzyText(ModifierCategory.Pseudo, pattern.Text),
                    pattern: ComputePattern(pattern.Text.Split(':', 2).Last().Trim(), ModifierCategory.Pseudo))
                {
                    OptionText = pattern.OptionText,
                    Value = pattern.Value,
                });
            }

            var ids = specialPatterns.Select(x => x.Id).Distinct().ToList();
            Patterns[ModifierCategory.Pseudo].RemoveAll(x => ids.Contains(x.Id));
            Patterns[ModifierCategory.Pseudo].AddRange(specialPatterns);
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
                var patterns = match.Patterns.OrderByDescending(x => Fuzz.Ratio(fuzzyLine, x.FuzzyText));
                foreach (var pattern in patterns)
                {
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
            modifierLines = modifierLines.OrderBy(x => parsingItem.Text.IndexOf(x.Text)).ToList();

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

                    var patterns = MatchModifierPatterns(block, line, lineIndex);
                    if (!patterns.Any())
                    {
                        // If we reach this point we have not found the modifier through traditional Regex means.
                        // Text from the game sometimes differ from the text from the API. We do a fuzzy search here to find the most common text.
                        patterns = MatchModifierFuzzily(parsingItem, line);
                    }

                    if (!patterns.Any())
                    {
                        continue;
                    }

                    var maxLineCount = patterns.Max(x => x.LineCount);
                    lineIndex += maxLineCount - 1; // Increment the line index by one less of the pattern count. The default lineIndex++ will take care of the remaining increment.
                    yield return new ModifierMatch(block, block.Lines.Skip(lineIndex).Take(maxLineCount), patterns.ToList());
                    continue;
                }
            }
        }

        private IEnumerable<ModifierPattern> MatchModifierPatterns(ParsingBlock block, ParsingLine line, int lineIndex)
        {
            foreach (var pattern in Patterns.SelectMany(x => x.Value))
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
            if (line.Parsed
                || item.Metadata?.Category == Category.Flask)
            {
                yield break;
            }

            var fuzzyLine = CleanFuzzyText(line.Text);
            var fuzzies = new List<FuzzyResult>();

            Parallel.ForEach(FuzzyDictionary, (x) =>
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

        public bool IsMatch(string id, string text)
        {
            foreach (var patternGroup in Patterns)
            {
                var pattern = patternGroup.Value.FirstOrDefault(x => x.Id == id);
                if (pattern != null && pattern.Pattern != null && pattern.Pattern.IsMatch(text))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
