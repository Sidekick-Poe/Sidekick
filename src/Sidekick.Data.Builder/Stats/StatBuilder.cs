using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Data.Builder.Repoe;
using Sidekick.Data.Builder.Repoe.Models.Stats;
using Sidekick.Data.Builder.Stats.Hooks;
using Sidekick.Data.Builder.Trade.Models;
using Sidekick.Data.Fuzzy;
using Sidekick.Data.Languages;
using Sidekick.Data.Stats;
using Sidekick.Data.StatsInvariant;
using Sidekick.Data.Trade;
namespace Sidekick.Data.Builder.Stats;

public class StatBuilder(
    ILogger<StatBuilder> logger,
    IOptions<SidekickConfiguration> configuration,
    DataProvider dataProvider,
    RepoeDownloader repoeDownloader,
    IFuzzyService fuzzyService)
{
    public async Task Build(IGameLanguage language)
    {
        try
        {
            var patterns = new StatPatternBuilder(language, fuzzyService);

            await Build(GameType.PathOfExile1, language, patterns);
            await Build(GameType.PathOfExile2, language, patterns);
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

    private async Task Build(GameType game, IGameLanguage language, StatPatternBuilder patternBuilder)
    {
        var context = new StatBuilderContext(Game: game,
                                             Language: language,
                                             PatternBuilder: patternBuilder,
                                             RepoeStats: await repoeDownloader.ReadStatTranslations(game, language.Code),
                                             TradeDefinitions: await dataProvider.Read<List<TradeStatDefinition>>(game, DataType.TradeStats, language),
                                             InvariantDetails: await dataProvider.Read<StatsInvariantDetails>(game, DataType.StatsInvariant));

        List<BaseHook> hooks =
        [
            new IgnoreGameIds(),
            new LogbookBosses(context),
            new LogbookFactions(context),
            new IncursionRooms(context),
        ];

        var definitions = BuildGameStats(context).ToList();
        foreach (var hook in hooks) hook.OnAfterGameBuild(definitions);

        definitions.AddRange(BuildMissingTradeStats(context, definitions));
        foreach (var hook in hooks) hook.OnAfterTradeBuild(definitions);

        await dataProvider.Write(game, DataType.Stats, language, definitions);
    }

    private IEnumerable<StatDefinition> BuildGameStats(StatBuilderContext context)
    {
        foreach (var repoeStat in context.RepoeStats)
        {
            if (repoeStat.Languages == null) yield break;

            foreach (var language in repoeStat.Languages)
            {
                if (string.IsNullOrEmpty(language.Text)) continue;

                var text = context.PatternBuilder.GetText(language.Text);
                var value = GetValue(language);
                var tradeStats = GetTradeStatDefinitions(repoeStat, value).ToList();

                yield return new StatDefinition
                {
                    GameIds = repoeStat.Ids,
                    Source = DataSource.Game,
                    TradeIds = tradeStats.Count > 0 ? tradeStats : null,
                    Text = text,
                    Negate = language.Handlers?.Any(x => x.Contains("negate")) ?? false,
                    Pattern = context.PatternBuilder.GetPattern(language.Text),
                    Value = tradeStats.All(x => !x.HasStatOption()) ? value : null,
                    Lines = text.Split('\n').Length,
                };
            }
        }

        yield break;

        IEnumerable<string> GetTradeStatDefinitions(RepoeStatTranslation repoeStat, int? value)
        {
            if (repoeStat.TradeStats == null) yield break;

            foreach (var repoeTradeStat in repoeStat.TradeStats)
            {
                if (repoeTradeStat.Options is { Options.Count: > 0 })
                {
                    foreach (var repoeStatOption in repoeTradeStat.Options.Options)
                    {
                        if (repoeStatOption.Id == value) yield return $"{repoeTradeStat.Id}#{repoeStatOption.Id}";
                    }
                    continue;
                }

                var split = repoeTradeStat.Id.Split('|', 2);
                if (split.Length == 2 && int.TryParse(split[1], out var idDiscriminator))
                {
                    if (idDiscriminator == value) yield return repoeTradeStat.Id;
                    continue;
                }

                yield return repoeTradeStat.Id;
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
            if (condition is { Min: 100, Max: null }) return null; // Special case to remove values from modifiers like Curse on Hit that have a 100% chance to curse.

            return (int)condition.Min.Value;
        }
    }

    private IEnumerable<StatDefinition> BuildMissingTradeStats(StatBuilderContext context, List<StatDefinition> definitions)
    {
        var definedTradeStats = definitions
            .Where(x => x.TradeIds != null)
            .SelectMany(x => x.TradeIds!)
            .ToHashSet();

        foreach (var tradeDefinition in context.TradeDefinitions)
        {
            if (definedTradeStats.Contains(tradeDefinition.Id)) continue;

            yield return GetTradePattern(tradeDefinition);
        }

        yield break;

        StatDefinition GetTradePattern(TradeStatDefinition tradeStatDefinition)
        {
            var text = context.PatternBuilder.GetText(tradeStatDefinition.Text, tradeStatDefinition.OptionText);

            return new StatDefinition
            {
                GameIds = null,
                Source = DataSource.Trade,
                Text = text,
                FuzzyText = context.PatternBuilder.GetFuzzyText(tradeStatDefinition.Text, tradeStatDefinition.OptionText),
                TradeIds = [tradeStatDefinition.Id],
                Pattern = context.PatternBuilder.GetPattern(tradeStatDefinition.Text, tradeStatDefinition.OptionText, replacePatterns: true),
                Lines = text.Split('\n').Length,
            };
        }
    }
}
