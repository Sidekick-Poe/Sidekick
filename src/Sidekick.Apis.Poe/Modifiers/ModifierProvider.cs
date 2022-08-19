using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FuzzySharp;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Apis.Poe.Parser;
using Sidekick.Apis.Poe.Pseudo;
using Sidekick.Common.Cache;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Items.Modifiers;

namespace Sidekick.Apis.Poe.Modifiers
{
    public class ModifierProvider : IModifierProvider
    {
        private readonly IPseudoModifierProvider pseudoModifierProvider;
        private readonly ICacheProvider cacheProvider;
        private readonly IPoeTradeClient poeTradeClient;
        private readonly IEnglishModifierProvider englishModifierProvider;
        private readonly Regex ParseHashPattern = new("\\#");
        private readonly Regex NewLinePattern = new("(?:\\\\)*[\\r\\n]+");
        private readonly Regex HashPattern = new("\\\\#");
        private readonly Regex ParenthesesPattern = new("((?:\\\\\\ )*\\\\\\([^\\(\\)]*\\\\\\))");
        private readonly Regex CleanFuzzyPattern = new("[-+0-9%#]");
        private readonly Regex TrimPattern = new(@"\s+");
        private readonly Regex CleanOriginalTextPattern = new(" \\((?:implicit|enchant|crafted|veiled|fractured|scourge)\\)$");

        public ModifierProvider(
            IPseudoModifierProvider pseudoModifierProvider,
            ICacheProvider cacheProvider,
            IPoeTradeClient poeTradeClient,
            IEnglishModifierProvider englishModifierProvider)
        {
            this.pseudoModifierProvider = pseudoModifierProvider;
            this.cacheProvider = cacheProvider;
            this.poeTradeClient = poeTradeClient;
            this.englishModifierProvider = englishModifierProvider;
        }

        public Dictionary<ModifierCategory, List<ModifierPatternMetadata>> Patterns { get; set; } = new();
        public List<ModifierPatternMetadata> IncursionRoomPatterns { get; set; }
        public List<ModifierPatternMetadata> LogbookFactionPatterns { get; set; }

        public Dictionary<string, List<FuzzyEntry>> FuzzyDictionary { get; set; } = new();

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
                    var modifier = new ModifierPatternMetadata()
                    {
                        Category = modifierCategory,
                        Id = entry.Id,
                        IsOption = entry.Option?.Options?.Any() ?? false,
                    };

                    if (modifier.IsOption)
                    {
                        for (var i = 0; i < entry.Option.Options.Count; i++)
                        {
                            modifier.Patterns.Add(new ModifierPattern()
                            {
                                Text = ComputeOptionText(entry.Text, entry.Option.Options[i].Text),
                                FuzzyText = ComputeFuzzyText(modifierCategory, entry.Text, entry.Option.Options[i].Text),
                                Value = entry.Option.Options[i].Id,
                                Pattern = ComputePattern(entry.Text, modifierCategory, entry.Option.Options[i].Text),
                            });
                        }
                    }
                    else
                    {
                        modifier.Patterns.Add(new ModifierPattern()
                        {
                            Text = entry.Text,
                            FuzzyText = ComputeFuzzyText(modifierCategory, entry.Text),
                            Pattern = ComputePattern(entry.Text, modifierCategory),
                        });
                    }

                    patterns.Add(modifier);
                }

                Patterns.Add(modifierCategory, patterns);

                BuildFuzzyDictionary(patterns);
            }

            // Prepare special pseudo patterns
            IncursionRoomPatterns = Patterns[ModifierCategory.Pseudo].Where(x => englishModifierProvider.IncursionRooms.Contains(x.Id)).ToList();
            ComputeSpecialPseudoPattern(IncursionRoomPatterns);
            LogbookFactionPatterns = Patterns[ModifierCategory.Pseudo].Where(x => englishModifierProvider.LogbookFactions.Contains(x.Id)).ToList();
            ComputeSpecialPseudoPattern(LogbookFactionPatterns);
        }

        /// <inheritdoc/>
        public ModifierCategory GetModifierCategory(string apiId)
        {
            return apiId.Split('.').First() switch
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
                _ => ModifierCategory.Undefined,
            };
        }

        private void ComputeSpecialPseudoPattern(List<ModifierPatternMetadata> patterns)
        {
            foreach (var pattern in patterns)
            {
                var basePattern = pattern.Patterns.OrderBy(x => x.Value).First();
                pattern.Patterns.Add(new ModifierPattern()
                {
                    Text = basePattern.Text,
                    FuzzyText = ComputeFuzzyText(ModifierCategory.Pseudo, basePattern.Text),
                    OptionText = basePattern.OptionText,
                    Pattern = ComputePattern(basePattern.Text.Split(':', 2).Last().Trim(), ModifierCategory.Pseudo),
                    Value = basePattern.Value,
                });
            }
        }

        private Regex ComputePattern(string text, ModifierCategory? category = null, string optionText = null)
        {
            // The notes in parentheses are never translated by the game.
            // We should be fine hardcoding them this way.
            var suffix = category switch
            {
                ModifierCategory.Implicit => "(?:\\ \\(implicit\\))?",
                ModifierCategory.Enchant => "(?:\\ \\(enchant\\))?",
                ModifierCategory.Crafted => "(?:\\ \\(crafted\\))?",
                ModifierCategory.Veiled => "(?:\\ \\(veiled\\))?",
                ModifierCategory.Fractured => "(?:\\ \\(fractured\\))?",
                ModifierCategory.Scourge => "(?:\\ \\(scourge\\))?",
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

        private string ComputeFuzzyText(ModifierCategory category, string text, string optionText = null)
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

                    FuzzyDictionary[pattern.FuzzyText].Add(new FuzzyEntry()
                    {
                        Metadata = patternMetadata,
                        Pattern = pattern
                    });
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
            return string.Join('\n', optionLines);
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

                    var modifierLine = new ModifierLine()
                    {
                        Text = CleanOriginalTextPattern.Replace(line.Text, string.Empty),
                    };

                    List<Modifier> modifiers = new();

                    foreach (var patternGroup in Patterns)
                    {
                        foreach (var metadata in patternGroup.Value)
                        {
                            foreach (var pattern in metadata.Patterns)
                            {
                                // Multiline modifiers
                                if (pattern.LineCount > 1 && pattern.Pattern.IsMatch(string.Join('\n', block.Lines.Skip(lineIndex).Take(pattern.LineCount))))
                                {
                                    ParseMod(modifiers, metadata, pattern, string.Join('\n', block.Lines.Skip(lineIndex).Take(pattern.LineCount)));
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
                                    ParseMod(modifiers, metadata, pattern, line.Text);
                                    line.Parsed = true;
                                }
                            }
                        }
                    }

                    // If we reach this point we have not found the modifier through traditional Regex means.
                    // Text from the game sometimes differ from the text from the API. We do a fuzzy search here to find the most common text.
                    var fuzzyLine = CleanFuzzyText(line.Text);
                    if (!line.Parsed && parsingItem.Metadata.Category != Category.Flask)
                    {
                        var fuzzies = new List<FuzzyResult>();

                        Parallel.ForEach(FuzzyDictionary, (x) =>
                        {
                            var ratio = Fuzz.Ratio(fuzzyLine, x.Key, FuzzySharp.PreProcess.PreprocessMode.None);
                            if (ratio > 75)
                            {
                                fuzzies.Add(new FuzzyResult()
                                {
                                    Ratio = ratio,
                                    Entries = x.Value,
                                });
                            }
                        });

                        if (fuzzies.Any())
                        {
                            fuzzies = fuzzies
                                .OrderByDescending(x => x.Ratio)
                                .ToList();

                            foreach (var fuzzy in fuzzies)
                            {
                                foreach (var modifier in fuzzy.Entries)
                                {
                                    ParseMod(modifiers, modifier.Metadata, modifier.Pattern, line.Text);
                                }
                            }

                            modifierLine.IsFuzzy = true;
                            line.Parsed = true;
                        }
                    }
                    else
                    {
                        modifiers = modifiers
                            .OrderBy(x => x.Category == ModifierCategory.Pseudo) // Put pseudo mods at the end.
                            .ThenByDescending(x => Fuzz.Ratio(fuzzyLine, CleanFuzzyText(x.Text))).ToList();
                    }

                    modifierLine.Modifier = modifiers.FirstOrDefault();
                    modifierLine.Alternates = modifiers.Skip(1).ToList();

                    modifierLines.Add(modifierLine);
                }
            }

            // Check if we need to process special pseudo patterns
            // if (parsingItem.Metadata.Class == Common.Game.Items.Class.MiscMapItems) ParseModifiers(modifiers, IncursionRoomPatterns, parsingItem);
            // if (parsingItem.Metadata.Class == Common.Game.Items.Class.Logbooks) ParseModifiers(modifiers, LogbookFactionPatterns, parsingItem);

            // Order the mods by the order they appear on the item.
            modifierLines = modifierLines.OrderBy(x => parsingItem.Text.IndexOf(x.Text)).ToList();

            // Trim the beginning of modifier lines
            for (var i = 0; i < modifierLines.Count; i++)
            {
                if (modifierLines[i].Modifier == null)
                {
                    modifierLines.Remove(modifierLines[i]);
                    i--;
                }
            }

            // Trim the end of modifier lines
            for (var i = modifierLines.Count - 1; i >= 0; i--)
            {
                if (modifierLines[i].Modifier == null)
                {
                    modifierLines.Remove(modifierLines[i]);
                }
            }

            return modifierLines;
        }

        private void ParseMod(List<Modifier> modifiers, ModifierPatternMetadata metadata, ModifierPattern pattern, string text)
        {
            var modifier = new Modifier()
            {
                Id = metadata.Id,
                Text = pattern.Text,
                Category = metadata.Category,
            };

            if (metadata.IsOption)
            {
                modifier.OptionValue = pattern.Value.Value;
            }
            else if (pattern.Value.HasValue)
            {
                modifier.Values.Add(pattern.Value.Value);
            }
            else
            {
                var matches = new Regex("([-+0-9,.]+)").Matches(text);
                foreach (Match match in matches)
                {
                    if (double.TryParse(match.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue))
                    {
                        modifier.Values.Add(parsedValue);
                    }
                }
            }

            modifiers.Add(modifier);
        }

        public bool IsMatch(string id, string text)
        {
            ModifierPatternMetadata metadata = null;

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
