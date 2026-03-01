using System.Globalization;
using System.Text.RegularExpressions;
using FuzzySharp;
using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.ApiStats;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Fuzzy;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using Sidekick.Data.Stats;

namespace Sidekick.Apis.Poe.Trade.Parser.Stats;

public class StatParser
(
    ISettingsService settingsService,
    ICurrentGameLanguage currentGameLanguage,
    IStringLocalizer<PoeResources> resources,
    IApiStatsProvider apiStatsProvider,
    DataProvider dataProvider,
    IFuzzyService fuzzyService
) : IStatParser
{
    private static readonly Regex ParseCategoryPattern = new(@" \(([a-zA-Z]+)\)$", RegexOptions.Multiline);

    public int Priority => 300;

    private List<StatDefinition> Definitions { get; set; } = [];

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        Definitions = await dataProvider.Read<List<StatDefinition>>(game, DataType.Stats, currentGameLanguage.Language);
    }

    /// <inheritdoc/>
    public void Parse(Item item)
    {
        if (!ItemClassConstants.WithStats.Contains(item.Properties.ItemClass)) return;

        var stats = MatchStats().ToList();
        item.Stats.Clear();
        item.Stats.AddRange(stats);

        return;

        IEnumerable<Stat> MatchStats()
        {
            foreach (var block in item.Text.Blocks)
            {
                for (var lineIndex = 0; lineIndex < block.Lines.Count; lineIndex++)
                {
                    if (block.Lines[lineIndex].Parsed) continue;

                    var definitions = MatchDefinitions(block, lineIndex).ToList();
                    var matchFuzzily = definitions.Sum(x => x.TradeIds.Count) is 0;
                    if (matchFuzzily) definitions.AddRange(MatchDefinitionsFuzzily(block, lineIndex));
                    if (definitions.Count is 0) continue;

                    var maxLineCount = definitions.Select(x => x.LineCount).Max();
                    definitions = definitions.Where(x => x.LineCount == maxLineCount).ToList();

                    var lines = block.Lines.Skip(lineIndex).Take(maxLineCount).ToList();
                    lines.ForEach(x => x.Parsed = true);

                    yield return CreateStat(block, lines, definitions, matchFuzzily);
                }
            }
        }

        IEnumerable<StatDefinition> MatchDefinitions(TextBlock block, int lineIndex)
        {
            foreach (var definition in Definitions)
            {
                // Multiple line stats
                if (definition.LineCount > 1 && definition.Pattern.IsMatch(string.Join('\n', block.Lines.Skip(lineIndex).Take(definition.LineCount))))
                {
                    yield return definition;
                }

                // Single line stats
                if (definition.Pattern.IsMatch(block.Lines[lineIndex].Text))
                {
                    yield return definition;
                }
            }
        }

        List<StatDefinition> MatchDefinitionsFuzzily(TextBlock block, int lineIndex)
        {
            var singleText = fuzzyService.CleanFuzzyText(currentGameLanguage.Language, block.Lines[lineIndex].Text);

            string? doubleText = null;
            if (lineIndex < block.Lines.Count - 1)
            {
                doubleText = $"{block.Lines[lineIndex].Text} {block.Lines[lineIndex + 1].Text}";
                doubleText = fuzzyService.CleanFuzzyText(currentGameLanguage.Language, doubleText);
            }

            var results = new List<(int Ratio, StatDefinition Definition)>();
            var resultsLock = new object();// Lock object to synchronize access to results

            Parallel.ForEach(Definitions,
                             definition =>
                             {
                                 if (string.IsNullOrEmpty(definition.FuzzyText)) return;

                                 var text = definition.LineCount switch
                                 {
                                     2 => doubleText ?? singleText,
                                     _ => singleText,
                                 };

                                 var ratio = Fuzz.Ratio(text, definition.FuzzyText, FuzzySharp.PreProcess.PreprocessMode.None);
                                 if (ratio <= 75)
                                 {
                                     return;
                                 }

                                 lock (resultsLock)// Lock before accessing the shared list
                                 {
                                     results.Add((ratio, definition));
                                 }
                             });

            if (results.Count == 0) return [];

            results = results.OrderByDescending(x => x.Ratio).ToList();
            var cutoff = results.First().Ratio - 2;
            return results.Where(x => x.Ratio > cutoff).Select(x => x.Definition).ToList();
        }

        Stat CreateStat(TextBlock block, List<TextLine> lines, List<StatDefinition> definitions, bool matchedFuzzily)
        {
            var text = string.Join('\n', lines.Select(x => x.Text));
            var category = ParseCategory(text);
            if (definitions.DistinctBy(x => x.Category).Count() == 1 && definitions[0].Category != StatCategory.Undefined)
            {
                category = definitions[0].Category;
            }

            text = RemoveCategory(text);

            var stat = new Stat(category, text)
            {
                BlockIndex = block.Index,
                LineIndex = lines.First().Index,
                Definitions = definitions,
                MatchedFuzzily = matchedFuzzily,
            };

            stat.Values = GetValues(stat).ToList();
            return stat;
        }
    }

    private StatCategory ParseCategory(string value)
    {
        var match = ParseCategoryPattern.Match(value);
        if (!match.Success)
        {
            return StatCategory.Explicit;
        }

        return match.Groups[1].Value.GetEnumFromValue<StatCategory>();
    }

    private string RemoveCategory(string value)
    {
        return ParseCategoryPattern.Replace(value, string.Empty);
    }

    private IEnumerable<double> GetValues(Stat stat)
    {
        var hardcodedDefinition = stat.Definitions.FirstOrDefault(x => x.Value.HasValue);
        if (hardcodedDefinition != null)
        {
            yield return hardcodedDefinition.Value!.Value;
            yield break;
        }

        foreach (var definition in stat.Definitions)
        {
            var patternMatch = definition.Pattern.Match(stat.Text);
            if (!patternMatch.Success) continue;

            var hasMatchingDescription = HasMatchingDescriptions(definition);
            var hasValues = false;
            foreach (Group group in patternMatch.Groups)
            {
                foreach (Capture capture in group.Captures)
                {
                    if (!double.TryParse(capture.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)) continue;
                    if (!hasMatchingDescription && definition.Negate) value *= -1;
                    yield return value;
                    hasValues = true;
                }
            }

            if (hasValues) yield break;
        }

        // Find numbers in the line
        if (stat.MatchedFuzzily)
        {
            var matches = new Regex("([-+0-9,.]+)").Matches(stat.Text);
            foreach (Match match in matches)
            {
                if (double.TryParse(match.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                {
                    yield return value;
                }
            }
        }

        yield break;

        bool HasMatchingDescriptions(StatDefinition definition)
        {
            foreach (var tradeId in definition.TradeIds)
            {
                var apiStats = apiStatsProvider.IdDictionary.GetValueOrDefault(tradeId);
                if (apiStats == null) continue;

                if (apiStats.Any(apiStat => apiStat.Text == definition.Text)) return true;
            }

            return false;
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
