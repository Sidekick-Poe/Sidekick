using System.Globalization;
using System.Text.RegularExpressions;
using FuzzySharp;
using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.ApiStats;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;
using Sidekick.Data.Fuzzy;
using Sidekick.Data.Trade.Models;
namespace Sidekick.Apis.Poe.Trade.Parser.Stats;

public class StatParser
(
    IFuzzyService fuzzyService,
    IApiStatsProvider apiStatsProvider,
    ISettingsService settingsService,
    ICurrentGameLanguage currentGameLanguage,
    IStringLocalizer<PoeResources> resources
) : IStatParser
{
    public int Priority => 300;

    private Regex? PositivePattern { get; set; }

    private Regex? NegativePattern { get; set; }

    public Task Initialize()
    {
        List<string> positiveTexts =
        [
            ..currentGameLanguage.Language.RegexIncreased.Split('|').Where(x => !string.IsNullOrWhiteSpace(x)),
            ..currentGameLanguage.Language.RegexMore.Split('|').Where(x => !string.IsNullOrWhiteSpace(x)),
            ..currentGameLanguage.Language.RegexFaster.Split('|').Where(x => !string.IsNullOrWhiteSpace(x)),
        ];
        PositivePattern = positiveTexts.Count != 0 ? new Regex($"(?:{string.Join('|', positiveTexts)})") : null;

        List<string> negativeTexts =
        [
            ..currentGameLanguage.Language.RegexReduced.Split('|').Where(x => !string.IsNullOrWhiteSpace(x)),
            ..currentGameLanguage.Language.RegexLess.Split('|').Where(x => !string.IsNullOrWhiteSpace(x)),
            ..currentGameLanguage.Language.RegexSlower.Split('|').Where(x => !string.IsNullOrWhiteSpace(x)),
        ];
        NegativePattern = negativeTexts.Count != 0 ? new Regex($"(?:{string.Join('|', negativeTexts)})") : null;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Parse(Item item)
    {
        if (!ItemClassConstants.WithStats.Contains(item.Properties.ItemClass)) return;

        var stats = MatchStats(item)
            // Trim stat lines
            .Where(x => x.ApiInformation.Count > 0)

            // Order the mods by the order they appear on the item.
            .OrderBy(x => x.BlockIndex)
            .ThenBy(x => x.LineIndex)
            .ToList();

        item.Stats.Clear();
        item.Stats.AddRange(stats);
    }

    private IEnumerable<Stat> MatchStats(Item item)
    {
        var allAvailablePatterns = GetAllAvailablePatterns(item);
        foreach (var block in item.Text.Blocks.Where(x => !x.AnyParsed))
        {
            for (var lineIndex = 0; lineIndex < block.Lines.Count; lineIndex++)
            {
                if (block.Lines[lineIndex].Parsed) continue;

                var definitions = MatchStatPatterns(block, lineIndex, allAvailablePatterns).ToList();
                var matchFuzzily = definitions.Count == 0;
                if (matchFuzzily)
                {
                    // If we reach this point we have not found the stat through traditional Regex means.
                    // Text from the game sometimes differ from the text from the API. We do a fuzzy search here to find the most common text.
                    definitions = MatchStatFuzzily(block, lineIndex, allAvailablePatterns).ToList();
                }

                if (definitions.Count is 0) continue;

                var maxLineCount = definitions.Max(x => x.LineCount);
                var lines = block.Lines.Skip(lineIndex).Take(maxLineCount).ToList();
                lines.ForEach(x => x.Parsed = true);

                yield return CreateStat(block, lines, definitions, matchFuzzily);
            }
        }
    }

    private static IEnumerable<TradeStatDefinition> MatchStatPatterns(TextBlock block, int lineIndex, IReadOnlyCollection<TradeStatDefinition> allAvailablePatterns)
    {
        foreach (var pattern in allAvailablePatterns)
        {
            // Multiple line stats
            if (pattern.LineCount > 1 && pattern.Pattern.IsMatch(string.Join('\n', block.Lines.Skip(lineIndex).Take(pattern.LineCount))))
            {
                yield return pattern;
                continue;
            }

            // Single line stats
            if (pattern.Pattern.IsMatch(block.Lines[lineIndex].Text))
            {
                yield return pattern;
            }
        }
    }

    private IEnumerable<TradeStatDefinition> MatchStatFuzzily(TextBlock block, int lineIndex, IReadOnlyCollection<TradeStatDefinition> allAvailablePatterns)
    {
        var category = block.Lines[lineIndex].Text.ParseCategory();
        if (category == StatCategory.Mutated) category = StatCategory.Explicit;

        var cleanLine = block.Lines[lineIndex].Text.RemoveCategory();
        var fuzzySingleLine = fuzzyService.CleanFuzzyText(currentGameLanguage.Language, cleanLine);
        string? fuzzyDoubleLine = null;
        if (lineIndex < block.Lines.Count - 1)
        {
            fuzzyDoubleLine = fuzzyService.CleanFuzzyText(currentGameLanguage.Language, cleanLine + " " + block.Lines[lineIndex + 1].Text);
        }

        var results = new List<(int Ratio, TradeStatDefinition Pattern)>();
        var resultsLock = new object();// Lock object to synchronize access to results

        Parallel.ForEach(allAvailablePatterns,
                         definition =>
                         {
                             if (category != StatCategory.Undefined && definition.Category != category) return;

                             if (category == StatCategory.Undefined && !StatCategoryExtensions.ExplicitCategories.Contains(definition.Category)) return;

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

    private IReadOnlyCollection<TradeStatDefinition> GetAllAvailablePatterns(Item item)
    {
        return item.Properties.ItemClass switch
        {
            ItemClass.SanctumRelic => apiStatsProvider.Definitions
                .Where(x => x.Category == StatCategory.Sanctum)
                .ToList(),
            ItemClass.Tablet => apiStatsProvider.Definitions
                .Where(x => x.Category == StatCategory.Implicit || x.Category == StatCategory.Explicit)
                .ToList(),
            _ => apiStatsProvider.Definitions.ToList(),
        };
    }

    private Stat CreateStat(TextBlock block, List<TextLine> lines, List<TradeStatDefinition> definitions, bool matchedFuzzily)
    {
        var text = string.Join('\n', lines.Select(x => x.Text));
        var category = text.ParseCategory();

        var stat = new Stat(text.RemoveCategory())
        {
            BlockIndex = block.Index,
            LineIndex = lines.First().Index,
            MatchedFuzzily = matchedFuzzily,
        };

        var fuzzyLine = fuzzyService.CleanFuzzyText(currentGameLanguage.Language, text);
        var filteredDefinitions = definitions
            .DistinctBy(x => x.Id)
            .OrderByDescending(x => Fuzz.Ratio(fuzzyLine, x.FuzzyText))
            .ToList();

        if (filteredDefinitions.Any(x => x.Category == StatCategory.Pseudo)
            && filteredDefinitions.Any(x => x.Category != StatCategory.Pseudo))
        {
            filteredDefinitions = filteredDefinitions.Where(x => x.Category != StatCategory.Pseudo).ToList();
        }

        foreach (var definition in filteredDefinitions)
        {
            stat.ApiInformation.Add(new(text: definition.Text)
            {
                Id = definition.Id,
                Category = category switch
                {
                    StatCategory.Mutated => StatCategory.Mutated,
                    _ => definition.Category,
                },
            });

            if (definition.SecondaryDefinitions != null)
            {
                foreach (var secondaryDefinition in definition.SecondaryDefinitions)
                {
                    stat.ApiInformation.Add(new(text: secondaryDefinition.Text)
                    {
                        Id = secondaryDefinition.Id,
                        Category = category switch
                        {
                            StatCategory.Mutated => StatCategory.Mutated,
                            _ => secondaryDefinition.Category,
                        },
                    });
                }
            }
        }

        ParseStatValue(stat, filteredDefinitions.FirstOrDefault());

        var originallyPositive = false;
        var negative = NegativePattern?.IsMatch(text) ?? false;
        foreach (var definition in definitions)
        {
            originallyPositive |= PositivePattern?.IsMatch(definition.Text) ?? false;
        }

        if (negative && originallyPositive)
        {
            var nagativeValues = stat.Values.Select(x => x * -1).ToList();
            stat.Values.Clear();
            stat.Values.AddRange(nagativeValues);
        }

        return stat;
    }

    private static void ParseStatValue(Stat stat, TradeStatDefinition? definition)
    {
        switch (definition)
        {
            case
            {
                IsOption: true
            }:
                stat.OptionValue = definition.OptionId;
                return;

            case
            {
                OptionId: int value,
            }:
                stat.Values.Add(value);
                return;

            case null: return;
        }

        // We try to parse the value from the line itself, if that fails we try to parse it from finding numbers in the line.
        var patternMatch = definition.Pattern.Match(stat.Text);
        if (patternMatch.Success)
        {
            foreach (Group group in patternMatch.Groups)
            {
                foreach (Capture capture in group.Captures)
                {
                    if (double.TryParse(capture.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue))
                    {
                        stat.Values.Add(parsedValue);
                    }
                }
            }

            return;
        }

        // Find numbers in the line
        var lines = stat.Text.Split('\n');
        foreach (var line in lines)
        {
            if (stat.Values.Count != 0) continue;

            var matches = new Regex("([-+0-9,.]+)").Matches(line);
            foreach (Match match in matches)
            {
                if (double.TryParse(match.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue))
                {
                    stat.Values.Add(parsedValue);
                }
            }
        }
    }

    public async Task<List<TradeFilter>> GetFilters(Item item)
    {
        if (!ItemClassConstants.WithStats.Contains(item.Properties.ItemClass)) return [];

        var autoSelectKey = $"Trade_Filter_Stat_{item.Game.GetValueAttribute()}";

        var result = new List<TradeFilter>();
        for (var i = 0; i < item.Stats.Count; i++)
        {
            result.Add(new StatFilter(item.Stats[i], item.Game)
            {
                AutoSelectSettingKey = autoSelectKey,
            });

            var isLastFilter = i + 1 == item.Stats.Count;
            if (isLastFilter) continue;

            var isDifferentBlock = item.Stats[i].BlockIndex != item.Stats[i + 1].BlockIndex;
            if (isDifferentBlock) result.Add(new SeparatorFilter());
        }

        var expandableFilter =
            new ExpandableFilter(resources["Stat_Filters"], result.ToArray())
            {
                AutoSelectSettingKey = autoSelectKey,
                DefaultAutoSelect = StatFilter.GetDefault(item.Game),
                Checked = true,
            };
        await expandableFilter.Initialize(item, settingsService);
        expandableFilter.Checked = true;

        return [expandableFilter];
    }
}
