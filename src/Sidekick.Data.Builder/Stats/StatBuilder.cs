using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Data.Builder.Repoe;
using Sidekick.Data.Builder.Repoe.Models.Poe1;
using Sidekick.Data.Builder.Trade;
using Sidekick.Data.Extensions;
using Sidekick.Data.Fuzzy;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using Sidekick.Data.Stats;
using Sidekick.Data.StatsInvariant;
namespace Sidekick.Data.Builder.Stats;

public class StatBuilder(
    ILogger<StatBuilder> logger,
    IOptions<SidekickConfiguration> configuration,
    DataProvider dataProvider,
    RepoeDownloader repoeDownloader,
    TradeStatProvider tradeStatProvider,
    IFuzzyService fuzzyService)
{
    private record TradeReplaceEntry(Regex Pattern, string Replacement);

    private readonly List<string> ignoredGameIds =
    [
        "map_set_league_category",
        "map_item_level_override",
    ];

    private readonly Regex textHashPattern = new(@"\#");
    private readonly Regex textGameHashPattern = new(@"\{\d+}");
    private readonly Regex textLocalPattern = new(@"\ \(Local\)$");

    private readonly Regex newLinePattern = new(@"(?:\\)*[\r\n]+");
    private readonly Regex hashPattern = new(@"\\#");
    private readonly Regex gameHashPattern = new(@"\\{\d+}");
    private readonly Regex parenthesesPattern = new(@"(\\\ *\\\([^\(\)]*\\\))");

    private List<TradeReplaceEntry> TradeReplacePatterns { get; set; } = [];

    public async Task Build(IGameLanguage language)
    {
        try
        {
            TradeReplacePatterns = BuildReplacementPatterns(language);

            await Build(GameType.PathOfExile1, language);
            await Build(GameType.PathOfExile2, language);
        }
        catch (Exception ex)
        {
            if (configuration.Value.ApplicationType == SidekickApplicationType.DataBuilder || configuration.Value.ApplicationType == SidekickApplicationType.Test)
            {
                throw;
            }

            logger.LogError(ex, $"Failed to build stats for {language.Code}.");
        }
    }

    private async Task Build(GameType game, IGameLanguage language)
    {
        var tradeDefinitions = await tradeStatProvider.GetDefinitions(game, language);

        var definitions = await BuildGameStats(game, language, tradeDefinitions);
        definitions.AddRange(BuildTradeStats(language, tradeDefinitions, definitions));

        var invariantStats = await dataProvider.Read<StatsInvariantDetails>(game, DataType.StatsInvariant);
        ComputeSpecialPseudoPattern(definitions, invariantStats.IncursionRoomStatIds);
        ComputeSpecialPseudoPattern(definitions, invariantStats.LogbookFactionStatIds);

        // Remove 'Closed Room' (2) options. We keep only 'Open Room' (1) options
        RemoveSpecialPseudoPattern(definitions, invariantStats.IncursionRoomStatIds, x => x.TradeStats.All(y => y.Option?.Id == 2));
        await dataProvider.Write(game, DataType.Stats, language, definitions);
    }

    private async Task<List<StatDefinition>> BuildGameStats(GameType game, IGameLanguage language, List<TradeStatDefinition> tradeDefinitions)
    {
        var gameStats = await repoeDownloader.ReadStatTranslations(game, language.Code);
        var definitions = new List<StatDefinition>();
        foreach (var gameStat in gameStats)
        {
            definitions.AddRange(GetGamePatterns(gameStat, tradeDefinitions));
        }

        return definitions;
    }

    private IEnumerable<StatDefinition> BuildTradeStats(IGameLanguage language, List<TradeStatDefinition> tradeDefinitions, List<StatDefinition> definitions)
    {
        foreach (var tradeDefinition in tradeDefinitions)
        {
            if (IsDefined(tradeDefinition)) continue;

            yield return GetTradePattern(language, tradeDefinition);
        }

        yield break;

        bool IsDefined(TradeStatDefinition tradeStatDefinition)
        {
            return (from definition in definitions
                from tradeStat in definition.TradeStats
                where tradeStat.Id == tradeStatDefinition.Id
                where tradeStat.Option?.Id == tradeStatDefinition.Option?.Id
                select true)
                .FirstOrDefault();

        }

        StatDefinition GetTradePattern(IGameLanguage gameLanguage, TradeStatDefinition tradeDefinition)
        {
            var text = GetText(tradeDefinition.Text, tradeDefinition.Option?.Text);

            return new StatDefinition()
            {
                Source = StatSource.Trade,
                Text = text,
                FuzzyText = GetFuzzyText(gameLanguage, tradeDefinition.Text, tradeDefinition.Option?.Text),
                TradeStats = [tradeDefinition],
                Pattern = GetPattern(tradeDefinition.Text, tradeDefinition.Option?.Text, replacePatterns: true),
                Lines = text.Split('\n').Length,
            };
        }
    }

    private IEnumerable<StatDefinition> GetGamePatterns(RepoeStatTranslation gameStat, List<TradeStatDefinition> tradeDefinitions)
    {
        if (gameStat.Languages == null || gameStat.Ids.Any(x => ignoredGameIds.Contains(x))) yield break;

        foreach (var language in gameStat.Languages)
        {
            if (string.IsNullOrEmpty(language.Text)) continue;

            if (language.Text == "Maximum number of Summoned Skeletons is Doubled\\nCannot have Minions other than Summoned Skeletons")
            {
                Debugger.Break();
            }

            var text = GetText(language.Text);
            var value = GetValue(language);
            var tradeStats = GetTradeStatDefinitions(value).ToList();

            yield return new StatDefinition()
            {
                Source = StatSource.Game,
                GameIds = gameStat.Ids,
                TradeStats = tradeStats,
                Text = text,
                Negate = language.Handlers?.Any(x => x.Contains("negate")) ?? false,
                Pattern = GetPattern(language.Text),
                Value = KeepValue(language, tradeStats) ? value : null,
                Lines = text.Split('\n').Length,
            };
        }

        yield break;

        IEnumerable<TradeStatDefinition> GetTradeStatDefinitions(int? value)
        {
            foreach (var repoeStat in gameStat.TradeStats)
            {
                foreach (var tradeDefinition in tradeDefinitions)
                {
                    if (tradeDefinition.Id != repoeStat.Id) continue;

                    if (repoeStat.Options is { Options.Count: > 0 })
                    {
                        foreach (var repoeStatOption in repoeStat.Options.Options)
                        {
                            if (tradeDefinition.Option == null || repoeStatOption.Id != tradeDefinition.Option.Id) continue;
                            if (repoeStatOption.Id == value) yield return tradeDefinition;
                        }
                        continue;
                    }

                    if (tradeDefinition.Option?.Id == repoeStat.OptionValue) yield return tradeDefinition;
                }
            }
        }

        int? GetValue(RepoeStatLanguage entry)
        {
            if (entry.Conditions?.Count != 1) return null;

            if (entry.Format?.Count != 1) return null;
            if (entry.Format[0] != "ignore") return null;

            var condition = entry.Conditions[0];
            if (!condition.Min.HasValue) return null;
            if (condition.Max.HasValue && Math.Abs(condition.Min.Value - condition.Max.Value) > 0.1) return null;

            return (int)condition.Min.Value;
        }

        bool KeepValue(RepoeStatLanguage entry, IList<TradeStatDefinition> tradeStats)
        {
            return tradeStats.All(x => x.Text != entry.Text);
        }
    }

    private string GetText(string text, string? optionText = null)
    {
        text = text.RemoveSquareBrackets();
        text = textGameHashPattern.Replace(text, "#");
        text = textLocalPattern.Replace(text, string.Empty);
        if (optionText == null) return text;

        optionText = optionText.RemoveSquareBrackets();

        var optionLines = new List<string>();
        foreach (var optionLine in newLinePattern.Split(optionText))
        {
            optionLines.Add(textHashPattern.Replace(text, optionLine));
        }

        return string.Join('\n', optionLines).Trim('\r', '\n');
    }

    private string GetFuzzyText(IGameLanguage language, string text, string? optionText = null)
    {
        if (string.IsNullOrEmpty(optionText))
        {
            return fuzzyService.CleanFuzzyText(language, text);
        }

        foreach (var optionLine in newLinePattern.Split(optionText))
        {
            text = textHashPattern.Replace(text, optionLine);
        }

        return fuzzyService.CleanFuzzyText(language, text);
    }

    private Regex GetPattern(string text, string? optionText = null, bool replacePatterns = false)
    {
        text = text.RemoveSquareBrackets();
        text = textLocalPattern.Replace(text, string.Empty);

        var suffix = @"(?:\ \([a-z]+\))?";

        var patternValue = Regex.Escape(text);
        patternValue = newLinePattern.Replace(patternValue, @"\n");
        patternValue = parenthesesPattern.Replace(patternValue, "(?:$1)?");

        if (string.IsNullOrEmpty(optionText))
        {
            patternValue = hashPattern.Replace(patternValue, @"([-+0-9,.]+)");
            patternValue = gameHashPattern.Replace(patternValue, @"([-+0-9,.]+)");
        }
        else
        {
            var optionLines = new List<string>();
            foreach (var optionLine in newLinePattern.Split(optionText))
            {
                optionLines.Add(hashPattern.Replace(patternValue, Regex.Escape(optionLine)) + suffix);
            }

            patternValue = string.Join('\n', optionLines.Where(x => !string.IsNullOrEmpty(x)));
        }

        if (replacePatterns)
        {
            foreach (var replaceEntry in TradeReplacePatterns)
            {
                patternValue = replaceEntry.Pattern.Replace(patternValue, replaceEntry.Replacement);
            }
        }

        patternValue = patternValue.Replace(@"\n", suffix + @"\n");// For multiline stats, the category can be suffixed on all lines.
        patternValue += suffix;

        return new Regex($"^{patternValue}$", RegexOptions.None);
    }

    private List<TradeReplaceEntry> BuildReplacementPatterns(IGameLanguage language)
    {
        List<TradeReplaceEntry> result = [];
        if (!string.IsNullOrEmpty(language.RegexIncreased))
        {
            result.Add(new(
                       new Regex(language.RegexIncreased),
                       $"(?:{language.RegexIncreased}|{language.RegexReduced})"));
        }

        if (!string.IsNullOrEmpty(language.RegexMore))
        {
            result.Add(new(
                       new Regex(language.RegexMore),
                       $"(?:{language.RegexMore}|{language.RegexLess})"));
        }

        if (!string.IsNullOrEmpty(language.RegexFaster))
        {
            result.Add(new(
                       new Regex(language.RegexFaster),
                       $"(?:{language.RegexFaster}|{language.RegexSlower})"));
        }

        return result;
    }

    private void ComputeSpecialPseudoPattern(List<StatDefinition> definitions, List<string> patternIds)
    {
        var patterns = (from definition in definitions
            from tradeStat in definition.TradeStats
            where tradeStat.Category == StatCategory.Pseudo
            where patternIds.Contains(tradeStat.Id)
            select definition);

        foreach (var pattern in patterns)
        {
            pattern.Pattern = GetPattern(pattern.Text.Split(':', 2).Last().Trim());
        }
    }

    private void RemoveSpecialPseudoPattern(List<StatDefinition> definitions, List<string> patternIds, Func<StatDefinition, bool> predicate)
    {
        definitions.RemoveAll(x => x.TradeStats.Any(y => patternIds.Contains(y.Id)) && predicate(x));
    }
}
