using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Items.Models;
using Sidekick.Data.Languages;
using Sidekick.Data.Stats.Models;

namespace Sidekick.Apis.Poe.Trade.Parser.Stats;

public class StatParser
(
    ISettingsService settingsService,
    ICurrentGameLanguage currentGameLanguage,
    IStringLocalizer<PoeResources> resources,
    DataProvider dataProvider
) : IStatParser
{
    private static readonly Regex ParseCategoryPattern = new(@" \(([a-zA-Z]+)\)$", RegexOptions.Multiline);

    public int Priority => 300;

    private List<StatPattern> Definitions { get; set; } = [];

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        Definitions = await dataProvider.Read<List<StatPattern>>(game, $"stats/{currentGameLanguage.Language.Code}.json");
    }

    /// <inheritdoc/>
    public void Parse(Item item)
    {
        if (!ItemClassConstants.WithStats.Contains(item.Properties.ItemClass)) return;

        var stats = MatchGamePatterns(item)
            // Trim stat lines
            .Where(x => x.MatchedPatterns.Count > 0)

            // Order the mods by the order they appear on the item.
            .OrderBy(x => x.BlockIndex)
            .ThenBy(x => x.LineIndex)
            .ToList();

        item.Stats.Clear();
        item.Stats.AddRange(stats);
    }

    private IEnumerable<Stat> MatchGamePatterns(Item item)
    {
        foreach (var block in item.Text.Blocks)
        {
            for (var lineIndex = 0; lineIndex < block.Lines.Count; lineIndex++)
            {
                if (block.Lines[lineIndex].Parsed) continue;

                var matchedPatterns = Match(block, lineIndex).ToList();
                if (matchedPatterns.Count is 0) continue;

                var maxLineCount = matchedPatterns.Select(x => x.LineCount).Max();
                matchedPatterns = matchedPatterns.Where(x => x.LineCount == maxLineCount).ToList();

                var lines = block.Lines.Skip(lineIndex).Take(maxLineCount).ToList();
                lines.ForEach(x => x.Parsed = true);

                yield return CreateStat(block, lines, matchedPatterns);
            }
        }

        yield break;

        IEnumerable<StatPattern> Match(TextBlock block, int lineIndex)
        {
            foreach (var pattern in Definitions)
            {
                // Multiple line stats
                if (pattern.LineCount > 1 && pattern.Pattern.IsMatch(string.Join('\n', block.Lines.Skip(lineIndex).Take(pattern.LineCount))))
                {
                    yield return pattern;
                }

                // Single line stats
                if (pattern.Pattern.IsMatch(block.Lines[lineIndex].Text))
                {
                    yield return pattern;
                }
            }
        }

        Stat CreateStat(TextBlock block, List<TextLine> lines, List<StatPattern> matchedPatterns)
        {
            var text = string.Join('\n', lines.Select(x => x.Text));
            var category = ParseCategory(text);
            text = RemoveCategory(text);

            var stat = new Stat(category, text)
            {
                BlockIndex = block.Index,
                LineIndex = lines.First().Index,
                MatchedPatterns = matchedPatterns,
            };

            stat.Values = GetValues(stat).ToList();
            return stat;

            StatCategory ParseCategory(string value)
            {
                var match = ParseCategoryPattern.Match(value);
                if (!match.Success)
                {
                    return StatCategory.Explicit;
                }

                return match.Groups[1].Value.GetEnumFromValue<StatCategory>();
            }

            string RemoveCategory(string value)
            {
                return ParseCategoryPattern.Replace(value, string.Empty);
            }

        }
    }

    private IEnumerable<double> GetValues(Stat stat)
    {
        foreach (var matchedPattern in stat.MatchedPatterns)
        {
            if (matchedPattern.Value.HasValue)
            {
                yield return matchedPattern.Value.Value;
                continue;
            }

            var patternMatch = matchedPattern.Pattern.Match(stat.Text);
            if (patternMatch.Success)
            {
                foreach (Group group in patternMatch.Groups)
                {
                    foreach (Capture capture in group.Captures)
                    {
                        if (double.TryParse(capture.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue))
                        {
                            if (matchedPattern.Negate) parsedValue *= -1;
                            yield return parsedValue;
                        }
                    }
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
