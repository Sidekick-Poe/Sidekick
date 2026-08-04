using System.Globalization;
using System.Text.RegularExpressions;
using FuzzySharp;
using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Extensions;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using Sidekick.Data.Stats;
using Sidekick.Data.StatsInvariant;
using Sidekick.Data.Trade;
using TradeFilter=Sidekick.Apis.Poe.Trade.Filters.Types.TradeFilter;

namespace Sidekick.Apis.Poe.Trade.Parser.Stats;

public class StatParser
(
    ISettingsService settingsService,
    ICurrentGameLanguage currentGameLanguage,
    IStringLocalizer<PoeResources> resources,
    DataProvider dataProvider
) : IStatParser
{
    public int Priority => 300;

    public StatsInvariantDetails InvariantDetails { get; private set; } = new();

    private Dictionary<StatCategory, List<StatDefinition>> Definitions { get; set; } = [];
    private List<StatDefinition> InvariantDefinitions { get; set; } = [];
    public Dictionary<string, List<TradeStatDefinition>> TradeDefinitions { get; private set; } = [];

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        var definitions = await dataProvider.Read<List<StatDefinition>>(game, DataType.Stats, currentGameLanguage.Language);
        foreach (var definition in definitions)
        {
            if (definition.TradeIds == null || definition.TradeIds.Count == 0)
            {
                Definitions.TryAdd(StatCategory.Undefined, []);
                Definitions[StatCategory.Undefined].Add(definition);
                continue;
            }

            foreach (var tradeId in definition.TradeIds)
            {
                var category = tradeId.GetStatCategory();
                Definitions.TryAdd(category, []);
                Definitions[category].Add(definition);
            }
        }

        var tradeDefinitions = await dataProvider.Read<List<TradeStatDefinition>>(game, DataType.TradeStats, currentGameLanguage.Language);
        TradeDefinitions = tradeDefinitions.GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x.ToList());

        InvariantDefinitions = await dataProvider.Read<List<StatDefinition>>(game, DataType.Stats, currentGameLanguage.InvariantLanguage);

        InvariantDetails = await dataProvider.Read<StatsInvariantDetails>(game, DataType.StatsInvariant);
    }

    public Stat? ParseInvariant(StatCategory category, string? line)
    {
        if (string.IsNullOrEmpty(line)) return null;

        var definitions = MatchDefinitions(InvariantDefinitions, [line]).ToList();
        if (definitions.Count == 0) return null;

        var maxLineCount = definitions.Select(x => x.Lines).Max();
        definitions = definitions.Where(x => x.Lines == maxLineCount).ToList();

        return CreateStat(category, line, definitions);
    }

    /// <inheritdoc/>
    public void Parse(Item item)
    {
        if (item.Properties.Rarity != Rarity.Normal &&
            item.Properties.Rarity != Rarity.Magic &&
            item.Properties.Rarity != Rarity.Rare &&
            item.Properties.Rarity != Rarity.Unique &&
            item.Properties.Rarity != Rarity.Gem &&
            item.Properties.Rarity != Rarity.Currency) return;

        foreach (var block in item.Text.Blocks)
        {
            if (block.AnyParsed) continue;

            var lineGroups = block.Lines.GroupBy(x => x.Category).ToList();
            foreach (var lineGroup in lineGroups)
            {
                var category = lineGroup.First().Category;
                var lines = lineGroup.Select(x => x.Text).ToList();

                for (var lineIndex = 0; lineIndex < lines.Count; lineIndex++)
                {
                    var definitions = MatchDefinitions(FilterDefinitions(item, category), lines.Skip(lineIndex))
                        .Distinct()
                        .ToList();
                    if (definitions.Count is 0) continue;

                    var maxLineCount = definitions.Select(x => x.Lines).Max();
                    definitions = definitions.Where(x => x.Lines == maxLineCount).ToList();

                    var matchedLines = lineGroup.Skip(lineIndex).Take(maxLineCount).ToList();
                    matchedLines.ForEach(x => x.Parsed = true);

                    var lineText = string.Join('\n', matchedLines.Select(x => x.Text));
                    definitions = definitions.OrderByDescending(x => Fuzz.Ratio(x.Text, lineText, FuzzySharp.PreProcess.PreprocessMode.None)).ToList();

                    item.Stats.Add(CreateStat(category, lineText, definitions, block.Index, matchedLines.First().Index));
                }
            }
        }

        item.Stats = item.Stats.OrderBy(x => x.BlockIndex).ThenBy(x => x.LineIndex).ToList();
    }

    private IEnumerable<StatDefinition> FilterDefinitions(Item item, StatCategory category)
    {
        if (item.Properties.Rarity == Rarity.Gem)
        {
            return Definitions
                .Where(x => x.Key
                           is StatCategory.Imbued)
                .SelectMany(x => x.Value);
        }

        if (category is StatCategory.Explicit or StatCategory.Mutated)
        {
            return Definitions
                .Where(x => x.Key
                           is StatCategory.Explicit
                           or StatCategory.Sanctum
                           or StatCategory.Pseudo)
                .SelectMany(x => x.Value);
        }

        if (category is StatCategory.Implicit)
        {
            return Definitions
                .Where(x => x.Key
                           is StatCategory.Implicit
                           or StatCategory.Pseudo)
                .SelectMany(x => x.Value);
        }

        if (category != StatCategory.Undefined)
        {
            return Definitions
                .Where(x => x.Key == category)
                .SelectMany(x => x.Value);
        }

        return Definitions.SelectMany(x => x.Value);
    }

    private IEnumerable<StatDefinition> MatchDefinitions(IEnumerable<StatDefinition> definitions, IEnumerable<string> lines)
    {
        foreach (var definition in definitions)
        {
            // Single line stats
            if (definition.Lines == 1 && definition.Pattern.IsMatch(lines.First()))
            {
                yield return definition;
            }

            // Multiple line stats
            if (definition.Lines > 1 && definition.Pattern.IsMatch(string.Join('\n', lines.Take(definition.Lines))))
            {
                yield return definition;
            }
        }
    }

    private Stat CreateStat(
        StatCategory category,
        string text,
        List<StatDefinition> definitions,
        int blockIndex = 0,
        int lineIndex = 0
    )
    {
        // Category overrides
        var categories = definitions
            .Where(x => x.TradeIds != null)
            .SelectMany(x => x.TradeIds!)
            .Select(x => x.GetStatCategory())
            .Distinct()
            .ToList();
        if (categories.Count == 1)
        {
            if (category == StatCategory.Explicit && categories[0] != StatCategory.Undefined)
                category = categories[0];
            if (category == StatCategory.Implicit && categories[0] == StatCategory.Pseudo)
                category = StatCategory.Pseudo;
        }

        var stat = new Stat(category, text)
        {
            BlockIndex = blockIndex,
            LineIndex = lineIndex,
            Definitions = definitions,
            HasTradeSupport = definitions.Any(x => x.TradeIds is { Count: > 0 }),
        };

        stat.Values = GetValues(stat).ToList();
        return stat;

        IEnumerable<double> GetValues(Stat input)
        {
            var hardcodedDefinition = input.Definitions.FirstOrDefault(x => x.Value.HasValue);
            if (hardcodedDefinition != null)
            {
                yield return hardcodedDefinition.Value!.Value;
                yield break;
            }

            foreach (var definition in input.Definitions)
            {
                var patternMatch = definition.Pattern.Match(input.Text);
                if (!patternMatch.Success) continue;

                var hasValues = false;
                foreach (Group group in patternMatch.Groups)
                {
                    foreach (Capture capture in group.Captures)
                    {
                        if (!double.TryParse(capture.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)) continue;
                        if (definition.Negate) value *= -1;
                        yield return value;
                        hasValues = true;
                    }
                }

                if (hasValues) yield break;
            }
        }
    }

    public async Task<List<TradeFilter>> GetFilters(Item item)
    {
        if (item.Properties.Rarity != Rarity.Normal &&
            item.Properties.Rarity != Rarity.Magic &&
            item.Properties.Rarity != Rarity.Rare &&
            item.Properties.Rarity != Rarity.Unique &&
            item.Properties.Rarity != Rarity.Gem) return [];

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
            new ExpandableFilter(resources["Stat_Filters"], true, result.ToArray())
            {
                AutoSelectSettingKey = autoSelectKey,
                DefaultAutoSelect = StatFilter.GetDefault(item.Game),
            };
        await expandableFilter.Initialize(item, settingsService);
        expandableFilter.Checked = true;

        return [expandableFilter];
    }
}
