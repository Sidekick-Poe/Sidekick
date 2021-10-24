using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Apis.Poe.Parser;
using Sidekick.Apis.Poe.Pseudo;
using Sidekick.Apis.Poe.Translations.Stats;
using Sidekick.Common.Cache;
using Sidekick.Common.Game.Items.Modifiers;

namespace Sidekick.Apis.Poe.Modifiers
{
    public class ModifierProvider : IModifierProvider
    {
        private readonly IPseudoModifierProvider pseudoModifierProvider;
        private readonly ICacheProvider cacheProvider;
        private readonly IPoeTradeClient poeTradeClient;
        private readonly IStatTranslationProvider statTranslationProvider;
        private readonly IEnglishModifierProvider englishModifierProvider;
        private readonly Regex ParseHashPattern = new("\\#");
        private readonly Regex NewLinePattern = new("(?:\\\\)*[\\r\\n]+");
        private readonly Regex HashPattern = new("\\\\#");
        private readonly Regex ParenthesesPattern = new("((?:\\\\\\ )*\\\\\\([^\\(\\)]*\\\\\\))");

        public ModifierProvider(
            IPseudoModifierProvider pseudoModifierProvider,
            ICacheProvider cacheProvider,
            IPoeTradeClient poeTradeClient,
            IStatTranslationProvider statTranslationProvider,
            IEnglishModifierProvider englishModifierProvider)
        {
            this.pseudoModifierProvider = pseudoModifierProvider;
            this.cacheProvider = cacheProvider;
            this.poeTradeClient = poeTradeClient;
            this.statTranslationProvider = statTranslationProvider;
            this.englishModifierProvider = englishModifierProvider;
        }

        public List<ModifierPatternMetadata> PseudoPatterns { get; set; }
        public List<ModifierPatternMetadata> ExplicitPatterns { get; set; }
        public List<ModifierPatternMetadata> ImplicitPatterns { get; set; }
        public List<ModifierPatternMetadata> EnchantPatterns { get; set; }
        public List<ModifierPatternMetadata> CraftedPatterns { get; set; }
        public List<ModifierPatternMetadata> VeiledPatterns { get; set; }
        public List<ModifierPatternMetadata> FracturedPatterns { get; set; }
        public List<ModifierPatternMetadata> ScourgePatterns { get; set; }
        public List<ModifierPatternMetadata> IncursionRoomPatterns { get; set; }

        public async Task Initialize()
        {
            await statTranslationProvider.Initialize();

            var result = await cacheProvider.GetOrSet(
                "ModifierProvider",
                () => poeTradeClient.Fetch<ApiCategory>("data/stats"));
            var categories = result.Result;

            PseudoPatterns = new List<ModifierPatternMetadata>();
            ExplicitPatterns = new List<ModifierPatternMetadata>();
            ImplicitPatterns = new List<ModifierPatternMetadata>();
            EnchantPatterns = new List<ModifierPatternMetadata>();
            CraftedPatterns = new List<ModifierPatternMetadata>();
            VeiledPatterns = new List<ModifierPatternMetadata>();
            FracturedPatterns = new List<ModifierPatternMetadata>();
            ScourgePatterns = new List<ModifierPatternMetadata>();

            foreach (var category in categories)
            {
                if (!category.Entries.Any())
                {
                    continue;
                }

                // The notes in parentheses are never translated by the game.
                // We should be fine hardcoding them this way.
                List<ModifierPatternMetadata> patterns;
                var categoryLabel = category.Entries[0].Id.Split('.').First();
                switch (categoryLabel)
                {
                    default: continue;
                    case "pseudo": patterns = PseudoPatterns; break;
                    case "delve":
                    case "monster":
                    case "explicit": patterns = ExplicitPatterns; break;
                    case "implicit": patterns = ImplicitPatterns; break;
                    case "enchant": patterns = EnchantPatterns; break;
                    case "crafted": patterns = CraftedPatterns; break;
                    case "veiled": patterns = VeiledPatterns; break;
                    case "fractured": patterns = FracturedPatterns; break;
                    case "scourge": patterns = ScourgePatterns; break;
                }

                foreach (var entry in category.Entries)
                {
                    var modifier = new ModifierPatternMetadata()
                    {
                        Category = categoryLabel,
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
                                Value = i + 1,
                                Pattern = ComputePattern(categoryLabel, entry.Text, entry.Option.Options[i].Text),
                            });
                        }
                    }
                    else
                    {
                        var stats = statTranslationProvider.GetAlternateModifiers(entry.Text);

                        if (stats != null)
                        {
                            foreach (var stat in stats)
                            {
                                modifier.Patterns.Add(new ModifierPattern()
                                {
                                    Text = stat.Text,
                                    LineCount = NewLinePattern.Matches(stat.Text).Count + 1,
                                    Pattern = ComputePattern(categoryLabel, stat.Text),
                                    Negative = stat.Negative,
                                    Value = stat.Value,
                                });
                            }
                        }
                        else
                        {
                            modifier.Patterns.Add(new ModifierPattern()
                            {
                                Text = entry.Text,
                                LineCount = NewLinePattern.Matches(entry.Text).Count + 1,
                                Pattern = ComputePattern(categoryLabel, entry.Text),
                            });
                        }
                    }

                    patterns.Add(modifier);
                }
            }

            // Prepare Incursion room patterns
            IncursionRoomPatterns = PseudoPatterns.Where(x => englishModifierProvider.IncursionRooms.Contains(x.Id)).ToList();
            foreach (var pattern in IncursionRoomPatterns)
            {
                var basePattern = pattern.Patterns.First(x => x.Value == 1);
                pattern.Patterns.Add(new ModifierPattern()
                {
                    LineCount = 1,
                    Negative = false,
                    Text = basePattern.Text,
                    OptionText = basePattern.OptionText,
                    Pattern = ComputePattern(string.Empty, basePattern.Text.Split(':', 2).Last().Trim()),
                    Value = basePattern.Value,
                });
            }
        }

        private Regex ComputePattern(string category, string text, string optionText = null)
        {
            // The notes in parentheses are never translated by the game.
            // We should be fine hardcoding them this way.
            var suffix = category switch
            {
                "implicit" => "(?:\\ \\(implicit\\))?",
                "enchant" => "(?:\\ \\(enchant\\))?",
                "crafted" => "(?:\\ \\(crafted\\))?",
                "veiled" => "(?:\\ \\(veiled\\))?",
                "fractured" => "(?:\\ \\(fractured\\))?",
                "scourge" => "(?:\\ \\(scourge\\))?",
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

        public ItemModifiers Parse(ParsingItem parsingItem)
        {
            var mods = new ItemModifiers();

            ParseModifiers(mods.Explicit, ExplicitPatterns, parsingItem);
            ParseModifiers(mods.Enchant, EnchantPatterns, parsingItem);
            ParseModifiers(mods.Implicit, ImplicitPatterns, parsingItem);
            ParseModifiers(mods.Crafted, CraftedPatterns, parsingItem);
            ParseModifiers(mods.Fractured, FracturedPatterns, parsingItem);
            ParseModifiers(mods.Scourge, ScourgePatterns, parsingItem);
            // ParseModifiers(mods.Veiled, VeiledPatterns, parsingItem);

            mods.Pseudo = pseudoModifierProvider.Parse(mods);

            // Check if we need to process Incursion room patterns
            if (parsingItem.Metadata.Class == Common.Game.Items.Class.MiscMapItems)
            {
                ParseModifiers(mods.Pseudo, IncursionRoomPatterns, parsingItem);
            }

            return mods;
        }

        private void ParseModifiers(List<Modifier> modifiers, List<ModifierPatternMetadata> metadatas, ParsingItem item)
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
                    }
                }
            }
        }

        private void ParseMod(List<Modifier> modifiers, ModifierPatternMetadata data, ModifierPattern pattern, string text)
        {
            var match = pattern.Pattern.Match(text);

            var modifier = new Modifier()
            {
                Index = match.Index,
                Id = data.Id,
                Text = pattern.Text,
                Category = !data.Id.StartsWith("explicit") ? data.Category : null,
            };

            if (data.IsOption)
            {
                modifier.OptionValue = new ModifierOption()
                {
                    Value = pattern.Value.ToString(),
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

            modifiers.Add(modifier);
        }

        public bool IsMatch(string id, string text)
        {
            ModifierPatternMetadata metadata = null;

            metadata = PseudoPatterns.FirstOrDefault(x => x.Id == id);
            if (metadata == null) metadata = ImplicitPatterns.FirstOrDefault(x => x.Id == id);
            if (metadata == null) metadata = ExplicitPatterns.FirstOrDefault(x => x.Id == id);
            if (metadata == null) metadata = CraftedPatterns.FirstOrDefault(x => x.Id == id);
            if (metadata == null) metadata = EnchantPatterns.FirstOrDefault(x => x.Id == id);
            if (metadata == null) metadata = FracturedPatterns.FirstOrDefault(x => x.Id == id);
            if (metadata == null) metadata = ScourgePatterns.FirstOrDefault(x => x.Id == id);


            if (metadata == null)
            {
                return false;
            }

            foreach (var pattern in metadata.Patterns)
            {
                if (pattern.Pattern != null && pattern.Pattern.IsMatch(text))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
