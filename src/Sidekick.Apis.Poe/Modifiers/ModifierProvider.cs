using System.Globalization;
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

        private Dictionary<ModifierCategory, List<ModifierPatternMetadata>> Patterns { get; } = new();
        private List<ModifierPatternMetadata> IncursionRoomPatterns { get; } = new();
        private List<ModifierPatternMetadata> LogbookFactionPatterns { get; } = new();
        private Dictionary<string, List<FuzzyEntry>> FuzzyDictionary { get; } = new();

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

                var patterns = new List<ModifierPatternMetadata>();
                foreach (var entry in category.Entries)
                {
                    if (entry.Text == null || entry.Id == null)
                    {
                        continue;
                    }

                    var modifier = new ModifierPatternMetadata(
                        category: modifierCategory,
                        id: entry.Id,
                        isOption: entry.Option?.Options?.Any() ?? false);

                    if (modifier.IsOption)
                    {
                        for (var i = 0; i < entry.Option?.Options.Count; i++)
                        {
                            var optionText = entry.Option.Options[i].Text;
                            if (optionText == null)
                            {
                                continue;
                            }

                            modifier.Patterns.Add(new ModifierPattern(
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
                        modifier.Patterns.Add(new ModifierPattern(
                            text: entry.Text,
                            fuzzyText: ComputeFuzzyText(modifierCategory, entry.Text),
                            pattern: ComputePattern(entry.Text, modifierCategory)));
                    }

                    patterns.Add(modifier);
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
            IncursionRoomPatterns.Clear();
            Patterns[ModifierCategory.Pseudo]
                .Where(x => englishModifierProvider.IncursionRooms.Contains(x.Id))
                .ToList()
                .ForEach(x =>
                {
                    IncursionRoomPatterns.Add(x);
                });
            ComputeSpecialPseudoPattern(IncursionRoomPatterns);

            LogbookFactionPatterns.Clear();
            Patterns[ModifierCategory.Pseudo]
                .Where(x => englishModifierProvider.LogbookFactions.Contains(x.Id))
                .ToList()
                .ForEach(x =>
                {
                    LogbookFactionPatterns.Add(x);
                });
            ComputeSpecialPseudoPattern(LogbookFactionPatterns);
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

        private void ComputeSpecialPseudoPattern(List<ModifierPatternMetadata> patterns)
        {
            foreach (var pattern in patterns)
            {
                var basePattern = pattern.Patterns.OrderBy(x => x.Value).First();
                pattern.Patterns.Add(new ModifierPattern(
                    text: basePattern.Text,
                    fuzzyText: ComputeFuzzyText(ModifierCategory.Pseudo, basePattern.Text),
                    pattern: ComputePattern(basePattern.Text.Split(':', 2).Last().Trim(), ModifierCategory.Pseudo))
                {
                    OptionText = basePattern.OptionText,
                    Value = basePattern.Value,
                });
            }
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

        private void BuildFuzzyDictionary(List<ModifierPatternMetadata> patternMetadatas)
        {
            foreach (var patternMetadata in patternMetadatas)
            {
                foreach (var pattern in patternMetadata.Patterns)
                {
                    if (!FuzzyDictionary.ContainsKey(pattern.FuzzyText))
                    {
                        FuzzyDictionary.Add(pattern.FuzzyText, new());
                    }

                    FuzzyDictionary[pattern.FuzzyText].Add(new FuzzyEntry(
                        metadata: patternMetadata,
                        pattern: pattern
                    ));
                }
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

            foreach (var block in parsingItem.Blocks.Where(x => !x.AnyParsed))
            {
                for (var lineIndex = 0; lineIndex < block.Lines.Count; lineIndex++)
                {
                    var line = block.Lines[lineIndex];
                    if (line.Parsed)
                    {
                        continue;
                    }

                    var modifierLine = new ModifierLine(
                        text: CleanOriginalTextPattern.Replace(line.Text, string.Empty));

                    MatchLineToPattern(modifierLine, block, line, ref lineIndex);
                    MatchLineWithFuzzy(parsingItem, modifierLine, line);

                    var fuzzyLine = CleanFuzzyText(line.Text);
                    modifierLine.Modifiers = modifierLine.Modifiers
                        .OrderBy(x => x.Category == ModifierCategory.Pseudo)
                        .ThenByDescending(x => Fuzz.Ratio(fuzzyLine, CleanFuzzyText(x.Text)))
                        .ToList();

                    modifierLines.Add(modifierLine);
                }
            }

            // Order the mods by the order they appear on the item.
            modifierLines = modifierLines.OrderBy(x => parsingItem.Text.IndexOf(x.Text)).ToList();

            // Trim modifier lines
            modifierLines.RemoveAll(x => x.Modifiers.Count == 0);

            return modifierLines;
        }

        private void MatchLineToPattern(ModifierLine modifierLine, ParsingBlock block, ParsingLine line, ref int lineIndex)
        {
            foreach (var metadata in Patterns.SelectMany(x => x.Value))
            {
                foreach (var pattern in metadata.Patterns)
                {
                    var isMultilineModifier = pattern.LineCount > 1 && pattern.Pattern.IsMatch(string.Join('\n', block.Lines.Skip(lineIndex).Take(pattern.LineCount)));
                    if (isMultilineModifier)
                    {
                        modifierLine.Modifiers.Add(ParseModifier(metadata, pattern));
                        ParseModifierValues(modifierLine, metadata, pattern, string.Join('\n', block.Lines.Skip(lineIndex).Take(pattern.LineCount)));
                        line.Parsed = true;

                        foreach (var multiline in block.Lines.Skip(lineIndex + 1).Take(pattern.LineCount - 1))
                        {
                            modifierLine.Text += $"\n{line.Text}";
                            multiline.Parsed = true;
                        }

                        // Increment the line index by one less of the pattern count. The default lineIndex++ will take care of the remaining increment.
                        lineIndex += pattern.LineCount - 1;
                    }
                    else if (pattern.Pattern.IsMatch(line.Text))
                    {
                        modifierLine.Modifiers.Add(ParseModifier(metadata, pattern));
                        ParseModifierValues(modifierLine, metadata, pattern, line.Text);
                        line.Parsed = true;
                    }
                }
            }
        }

        private void MatchLineWithFuzzy(ParsingItem parsingItem, ModifierLine modifierLine, ParsingLine line)
        {
            if (line.Parsed)
            {
                return;
            }

            if (parsingItem.Metadata?.Category == Category.Flask)
            {
                return;
            }

            // If we reach this point we have not found the modifier through traditional Regex means.
            // Text from the game sometimes differ from the text from the API. We do a fuzzy search here to find the most common text.
            var fuzzyLine = CleanFuzzyText(line.Text);
            var fuzzies = new List<FuzzyResult>();

            Parallel.ForEach(FuzzyDictionary, (x) =>
            {
                var ratio = Fuzz.Ratio(fuzzyLine, x.Key, FuzzySharp.PreProcess.PreprocessMode.None);
                if (ratio > 75)
                {
                    fuzzies.Add(new FuzzyResult(
                        ratio: ratio,
                        entries: x.Value
                    ));
                }
            });

            if (!fuzzies.Any())
            {
                return;
            }

            fuzzies = fuzzies
                .OrderByDescending(x => x.Ratio)
                .ToList();

            foreach (var fuzzy in fuzzies)
            {
                foreach (var modifier in fuzzy.Entries)
                {
                    ParseModifier(modifier.Metadata, modifier.Pattern);
                    ParseModifierValues(modifierLine, modifier.Metadata, modifier.Pattern, line.Text);
                }
            }

            modifierLine.IsFuzzy = true;
            line.Parsed = true;
        }

        private Modifier ParseModifier(ModifierPatternMetadata metadata, ModifierPattern pattern)
        {
            var modifier = new Modifier(text: pattern.Text)
            {
                Id = metadata.Id,
                Category = metadata.Category,
            };

            return modifier;
        }

        private void ParseModifierValues(ModifierLine modifierLine, ModifierPatternMetadata metadata, ModifierPattern pattern, string text)
        {
            if (metadata.IsOption)
            {
                modifierLine.OptionValue = pattern.Value;
            }

            if (pattern.Value.HasValue)
            {
                modifierLine.Values.Add(pattern.Value.Value);
            }
            else
            {
                var matches = new Regex("([-+0-9,.]+)").Matches(text);
                foreach (Match match in matches)
                {
                    if (double.TryParse(match.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue))
                    {
                        modifierLine.Values.Add(parsedValue);
                    }
                }
            }
        }

        public bool IsMatch(string id, string text)
        {
            ModifierPatternMetadata? metadata = null;

            foreach (var patternGroup in Patterns)
            {
                metadata = patternGroup.Value.FirstOrDefault(x => x.Id == id);
                if (metadata != null)
                {
                    foreach (var pattern in metadata.Patterns)
                    {
                        if (pattern.Pattern != null && pattern.Pattern.IsMatch(text))
                        {
                            return true;
                        }
                    }

                    break;
                }
            }

            return false;
        }
    }
}
