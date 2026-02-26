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
using Sidekick.Data.Fuzzy;
using Sidekick.Data.Items;
using Sidekick.Data.Items.Models;
using Sidekick.Data.Languages;
using Sidekick.Data.Stats;
using Sidekick.Data.Stats.Models;
using Sidekick.Data.Trade.Models;
namespace Sidekick.Apis.Poe.Trade.Parser.Stats;

public class StatParser
(
    IFuzzyService fuzzyService,
    ISettingsService settingsService,
    ICurrentGameLanguage currentGameLanguage,
    IStringLocalizer<PoeResources> resources,
    StatDataProvider statDataProvider
) : IStatParser
{
    public int Priority => 300;

    private List<StatDefinition> Definitions { get; set; } = [];

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        Definitions = await statDataProvider.GetStats(game, currentGameLanguage.Language.Code);
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

                var definitions = Match(block, lineIndex).ToList();
                if (definitions.Count is 0) continue;

                var maxLineCount = definitions.Max(x => x.LineCount);
                var lines = block.Lines.Skip(lineIndex).Take(maxLineCount).ToList();
                lines.ForEach(x => x.Parsed = true);

                yield return CreateStat(block, lines, definitions);
            }
        }

        yield break;

        IEnumerable<StatDefinition> Match(TextBlock block, int lineIndex)
        {
            foreach (var definition in Definitions)
            {
                var definitionMatched = false;

                foreach (var pattern in definition.GamePatterns)
                {
                    if (definitionMatched) continue;

                    // Multiple line stats
                    if (pattern.LineCount > 1 && pattern.Pattern.IsMatch(string.Join('\n', block.Lines.Skip(lineIndex).Take(pattern.LineCount))))
                    {
                        definitionMatched = true;
                        yield return definition;
                    }

                    // Single line stats
                    if (!definitionMatched && pattern.Pattern.IsMatch(block.Lines[lineIndex].Text))
                    {
                        definitionMatched = true;
                        yield return definition;
                    }

                    if (definitionMatched)
                    {
                        var lines = block.Lines.Skip(lineIndex).Take(pattern.LineCount).ToList();
                        lines.ForEach(x => x.Parsed = true);
                    }
                }
            }
        }

    }

    private Stat CreateStat(TextBlock block, List<TextLine> lines, List<StatDefinition> definitions)
    {
        var text = string.Join('\n', lines.Select(x => x.Text));
        var category = text.ParseCategory();

        var stat = new Stat(text.RemoveCategory())
        {
            BlockIndex = block.Index,
            LineIndex = lines.First().Index,
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
            stat.MatchedPatterns.Add(new(text: definition.Text)
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
                foreach (var secondaryDefinitionId in definition.SecondaryDefinitions)
                {
                    var secondaryDefinition = apiStatsProvider.Definitions.FirstOrDefault(x => x.Id == secondaryDefinitionId);
                    if (secondaryDefinition == null) continue;

                    stat.MatchedPatterns.Add(new(text: secondaryDefinition.Text)
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

        ParseValues(stat, filteredDefinitions.FirstOrDefault());

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

    private static void ParseValues(Stat stat)
    {
        foreach (var matchedPattern in stat.GetTradePatterns())
        {
            if (matchedPattern.Option != null)
            {
                stat.OptionId = matchedPattern.Option.Id;
                continue;
            }

            // We try to parse the value from the line itself, if that fails we try to parse it from finding numbers in the line.
            var patternMatch = matchedPattern.Pattern.Match(stat.Text);
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
