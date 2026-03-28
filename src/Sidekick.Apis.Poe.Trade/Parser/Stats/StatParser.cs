using System.Globalization;
using System.Text.RegularExpressions;
using FuzzySharp;
using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Fuzzy;
using Sidekick.Data.Languages;
using Sidekick.Data.Stats;
using Sidekick.Data.StatsInvariant;

namespace Sidekick.Apis.Poe.Trade.Parser.Stats;

public class StatParser
(
    ISettingsService settingsService,
    ICurrentGameLanguage currentGameLanguage,
    IStringLocalizer<PoeResources> resources,
    DataProvider dataProvider,
    IFuzzyService fuzzyService
) : IStatParser
{
    private static readonly Regex ParseCategoryPattern = new(@" \(([a-zA-Z]+)\)$", RegexOptions.Multiline);

    public int Priority => 300;

    public StatsInvariantDetails InvariantDetails { get; private set; } = new();

    private List<StatDefinition> Definitions { get; set; } = [];
    private List<StatDefinition> InvariantDefinitions { get; set; } = [];

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        Definitions = await dataProvider.Read<List<StatDefinition>>(game, DataType.Stats, currentGameLanguage.Language);

        if (currentGameLanguage.Language.Code == currentGameLanguage.InvariantLanguage.Code) InvariantDefinitions = Definitions;
        else InvariantDefinitions = await dataProvider.Read<List<StatDefinition>>(game, DataType.Stats, currentGameLanguage.InvariantLanguage);

        InvariantDetails = await dataProvider.Read<StatsInvariantDetails>(game, DataType.StatsInvariant);
    }

    /// <inheritdoc/>
    public void Parse(Item item)
    {
        if (!item.CanHaveStats) return;

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
                    var matchFuzzily = definitions.Sum(x => x.TradeStats.Count) is 0;
                    if (matchFuzzily) definitions.AddRange(MatchDefinitionsFuzzily(block, lineIndex));
                    if (definitions.Count is 0) continue;

                    var maxLineCount = definitions.Select(x => x.Lines).Max();
                    definitions = definitions.Where(x => x.Lines == maxLineCount).ToList();

                    var lines = block.Lines.Skip(lineIndex).Take(maxLineCount).ToList();
                    lines.ForEach(x => x.Parsed = true);

                    yield return CreateStat(block, lines, definitions, matchFuzzily);
                }
            }
        }

        IEnumerable<StatDefinition> FilterDefinitions()
        {
            return item.Properties.Rarity switch
            {
                Rarity.Gem => Definitions.Where(x => x.TradeStats.Any(y => y.Category is StatCategory.Imbued)),
                _ => Definitions,
            };
        }

        IEnumerable<StatDefinition> MatchDefinitions(RawBlock block, int lineIndex)
        {
            foreach (var definition in FilterDefinitions())
            {
                // Multiple line stats
                if (definition.Lines > 1 && definition.Pattern.IsMatch(string.Join('\n', block.Lines.Skip(lineIndex).Take(definition.Lines))))
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

        List<StatDefinition> MatchDefinitionsFuzzily(RawBlock block, int lineIndex)
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

            Parallel.ForEach(FilterDefinitions(),
                             definition =>
                             {
                                 if (string.IsNullOrEmpty(definition.FuzzyText)) return;

                                 var text = definition.Lines switch
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

            var orderedResults = results.OrderByDescending(x => x.Ratio).ToList();
            var cutoff = orderedResults.First().Ratio - 2;
            return orderedResults.Where(x => x.Ratio > cutoff).Select(x => x.Definition).ToList();
        }

        Stat CreateStat(RawBlock block, List<RawLine> lines, List<StatDefinition> definitions, bool matchedFuzzily)
        {
            var text = string.Join('\n', lines.Select(x => x.Text));
            var category = ParseCategory(text);
            if (category != StatCategory.Mutated)
            {
                var categories = definitions.SelectMany(x => x.TradeStats).Select(x => x.Category).Distinct().ToList();
                if (categories.Count == 1 && categories[0] != StatCategory.Undefined)
                {
                    category = categories[0];
                }
            }

            text = ParseCategoryPattern.Replace(text, string.Empty);

            var stat = new Stat(category, text)
            {
                BlockIndex = block.Index,
                LineIndex = lines.First().Index,
                Definitions = definitions,
                MatchedFuzzily = matchedFuzzily,
                HasTradeSupport = definitions.Any(x => x.TradeStats.Count > 0),
            };

            stat.Values = GetValues(stat).ToList();
            return stat;
        }

        StatCategory ParseCategory(string value)
        {
            var match = ParseCategoryPattern.Match(value);
            if (!match.Success) return StatCategory.Explicit;

            return match.Groups[1].Value.GetEnumFromValue<StatCategory>();
        }
    }

    public Stat? ParseInvariant(string? line)
    {
        if(string.IsNullOrEmpty(line)) return null;


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
            foreach (var tradeStat in definition.TradeStats)
            {
                if (tradeStat.Text == definition.Text) return true;
            }

            return false;
        }
    }

    public async Task<List<TradeFilter>> GetFilters(Item item)
    {
        if (!item.CanHaveStats) return [];

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
