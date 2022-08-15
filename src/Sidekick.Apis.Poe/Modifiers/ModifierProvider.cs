using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Apis.Poe.Parser;
using Sidekick.Apis.Poe.Pseudo;
using Sidekick.Common.Cache;
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

        public Dictionary<string, List<(ModifierPatternMetadata Metadata, ModifierPattern Pattern)>> FuzzyDictionary { get; set; } = new();

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
                                LineCount = NewLinePattern.Matches(entry.Text).Count + NewLinePattern.Matches(entry.Option.Options[i].Text).Count + 1,
                                Value = entry.Option.Options[i].Id,
                                Pattern = ComputePattern(modifierCategory, entry.Text, entry.Option.Options[i].Text),
                            });
                        }
                    }
                    else
                    {
                        modifier.Patterns.Add(new ModifierPattern()
                        {
                            Text = entry.Text,
                            LineCount = NewLinePattern.Matches(entry.Text).Count + 1,
                            Pattern = ComputePattern(modifierCategory, entry.Text),
                        });
                    }

                    patterns.Add(modifier);
                }

                Patterns.Add(modifierCategory, patterns);

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
                    LineCount = 1,
                    Negative = false,
                    Text = basePattern.Text,
                    OptionText = basePattern.OptionText,
                    Pattern = ComputePattern(ModifierCategory.Pseudo, basePattern.Text.Split(':', 2).Last().Trim()),
                    Value = basePattern.Value,
                });
            }
        }

        private Regex ComputePattern(ModifierCategory category, string text, string optionText = null)
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
                patternValue = HashPattern.Replace(patternValue, "([-+0-9,.]+)");
            }
            else
            {
                var optionLines = new List<string>();
                foreach (var optionLine in NewLinePattern.Split(optionText))
                {
                    optionLines.Add(HashPattern.Replace(patternValue, Regex.Escape(optionLine)));
                }
                patternValue = string.Join('\n', optionLines);
            }

            return new Regex($"^{patternValue}{suffix}$", RegexOptions.None);
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

        public List<ModifierLine> Parse(ParsingItem parsingItem)
        {
            var modifiers = new List<ModifierLine>();

            foreach (var patternGroup in Patterns)
            {
                if (modifiers == null)
                {
                    continue;
                }

                ParseModifiers(modifiers, patternGroup.Value, parsingItem);
            }

            // Check if we need to process special pseudo patterns
            if (parsingItem.Metadata.Class == Common.Game.Items.Class.MiscMapItems) ParseModifiers(modifiers, IncursionRoomPatterns, parsingItem);
            if (parsingItem.Metadata.Class == Common.Game.Items.Class.Logbooks) ParseModifiers(modifiers, LogbookFactionPatterns, parsingItem);

            // Order the mods by the order they appear on the item.
            return modifiers
                .OrderBy(x => parsingItem.Text.IndexOf(x.Text))
                .ToList();
        }

        private void ParseModifiers(List<ModifierLine> modifiers, List<ModifierPatternMetadata> metadatas, ParsingItem item)
        {
            foreach (var block in item.Blocks.Where(x => !x.Parsed))
            {
                foreach (var line in block.Lines.Where(x => !x.Parsed))
                {
                    foreach (var metadata in metadatas)
                    {
                        foreach (var pattern in metadata.Patterns)
                        {
                            if (pattern.Pattern.IsMatch(line.Text))
                            {
                                ParseMod(modifiers, metadata, pattern, line.Text);
                                line.Parsed = true;
                            }

                            // Multiline modifiers
                            else if (pattern.LineCount > 1 && pattern.Pattern.IsMatch(string.Join('\n', block.Lines.Skip(line.Index).Take(pattern.LineCount))))
                            {
                                ParseMod(modifiers, metadata, pattern, string.Join('\n', block.Lines.Skip(line.Index).Take(pattern.LineCount)));
                                foreach (var multiline in block.Lines.Skip(line.Index).Take(pattern.LineCount))
                                {
                                    multiline.Parsed = true;
                                }
                            }
                        }

                        // If we reach this point we have not found the modifier through traditional Regex means.
                        // Text from the game sometimes differ from the text from the API. We do a fuzzy search here to find the most common text.
                        if (!line.Parsed)
                        {
                            foreach (var pattern in metadata.Patterns)
                            {
                                // Fuzz
                            }
                        }
                    }
                }
            }
        }

        private void ParseMod(List<ModifierLine> modifiers, ModifierPatternMetadata data, ModifierPattern pattern, string text)
        {
            var match = pattern.Pattern.Match(text);

            var modifier = new Modifier()
            {
                Index = match.Index,
                Id = data.Id,
                Text = pattern.Text,
                Category = data.Category,
            };

            if (data.IsOption)
            {
                modifier.OptionValue = new ModifierOption()
                {
                    Value = pattern.Value.Value,
                };
            }
            else if (pattern.Value.HasValue)
            {
                modifier.Values.Add(pattern.Value.Value);
                modifier.Text = ParseHashPattern.Replace(modifier.Text, match.Groups[1].Value, 1);
            }
            else if (match.Groups.Count > 1)
            {
                for (var index = 1; index < match.Groups.Count; index++)
                {
                    if (double.TryParse(match.Groups[index].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue))
                    {
                        if (pattern.Negative)
                        {
                            parsedValue *= -1;
                        }

                        modifier.Values.Add(parsedValue);
                        modifier.Text = ParseHashPattern.Replace(modifier.Text, match.Groups[index].Value, 1);
                    }
                }

                modifier.Text = modifier.Text.Replace("+-", "-");
            }

            modifiers.Add(new ModifierLine()
            {
                Modifiers = new() { modifier },
                Text = text,
            });
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
