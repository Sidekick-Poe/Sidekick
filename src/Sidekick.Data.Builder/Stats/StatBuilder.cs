using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Data.Builder.Repoe;
using Sidekick.Data.Builder.Repoe.Models.Stats;
using Sidekick.Data.Builder.Stats.Hooks;
using Sidekick.Data.Builder.Trade;
using Sidekick.Data.Fuzzy;
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
    public async Task Build(IGameLanguage language)
    {
        try
        {
            var patterns = new StatPatterns(language, fuzzyService);

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

    private async Task Build(GameType game, IGameLanguage language, StatPatterns patterns)
    {
        var context = new StatBuilderContext(Game: game,
                                             Language: language,
                                             Patterns: patterns,
                                             TradeDefinitions: await tradeStatProvider.GetDefinitions(game, language),
                                             InvariantDetails: await dataProvider.Read<StatsInvariantDetails>(game, DataType.StatsInvariant));

        List<BaseHook> hooks =
        [
            new IgnoreGameIds(),
            new LogbookBosses(context),
            new LogbookFactions(context),
            new IncursionRooms(context),
        ];

        var definitions = await BuildGameStats(context);
        foreach (var hook in hooks) hook.OnAfterGameBuild(definitions);

        definitions.AddRange(BuildTradeStats(context, definitions));
        foreach (var hook in hooks) hook.OnAfterTradeBuild(definitions);

        await dataProvider.Write(game, DataType.Stats, language, definitions);
    }

    private async Task<List<StatDefinition>> BuildGameStats(StatBuilderContext context)
    {
        var gameStats = await repoeDownloader.ReadStatTranslations(context.Game, context.Language.Code);
        var definitions = new List<StatDefinition>();
        var tradeStatsById = context.TradeDefinitions.GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x.ToList());
        foreach (var gameStat in gameStats)
        {
            definitions.AddRange(GetGamePatterns(context, gameStat, tradeStatsById));
        }

        return definitions;
    }

    private IEnumerable<StatDefinition> BuildTradeStats(StatBuilderContext context, List<StatDefinition> definitions)
    {
        var definedTradeStats = definitions
            .Where(x => x.TradeStats != null)
            .SelectMany(x => x.TradeStats!)
            .Select(x => (x.Id, x.Option?.Id))
            .ToHashSet();

        foreach (var tradeDefinition in context.TradeDefinitions)
        {
            if (definedTradeStats.Contains((tradeDefinition.Id, tradeDefinition.Option?.Id))) continue;

            yield return GetTradePattern(tradeDefinition);
        }

        yield break;

        StatDefinition GetTradePattern(TradeStatDefinition tradeDefinition)
        {
            var text = context.Patterns.GetText(tradeDefinition.Text, tradeDefinition.Option?.Text);

            return new StatDefinition()
            {
                Source = DataSource.Trade,
                Text = text,
                FuzzyText = context.Patterns.GetFuzzyText(tradeDefinition.Text, tradeDefinition.Option?.Text),
                TradeStats = [tradeDefinition],
                Pattern = context.Patterns.GetPattern(tradeDefinition.Text, tradeDefinition.Option?.Text, replacePatterns: true),
                Lines = text.Split('\n').Length,
            };
        }
    }

    private IEnumerable<StatDefinition> GetGamePatterns(StatBuilderContext context, RepoeStatTranslation gameStat, Dictionary<string, List<TradeStatDefinition>> tradeDefinitionsById)
    {
        if (gameStat.Languages == null) yield break;

        foreach (var language in gameStat.Languages)
        {
            if (string.IsNullOrEmpty(language.Text)) continue;

            var text = context.Patterns.GetText(language.Text);
            var value = GetValue(language);
            var tradeStats = GetTradeStatDefinitions(value).ToList();

            yield return new StatDefinition()
            {
                Source = DataSource.Game,
                GameIds = gameStat.Ids.Count > 0 ? gameStat.Ids : null,
                TradeStats = tradeStats.Count > 0 ? tradeStats : null,
                Text = text,
                Negate = language.Handlers?.Any(x => x.Contains("negate")) ?? false,
                Pattern = context.Patterns.GetPattern(language.Text),
                Value = KeepValue(language, tradeStats) ? value : null,
                Lines = text.Split('\n').Length,
            };
        }

        yield break;

        IEnumerable<TradeStatDefinition> GetTradeStatDefinitions(int? value)
        {
            if (gameStat.TradeStats == null) yield break;

            foreach (var repoeStat in gameStat.TradeStats)
            {
                if (!tradeDefinitionsById.TryGetValue(repoeStat.Id, out var tradeDefinitions)) continue;

                foreach (var tradeDefinition in tradeDefinitions)
                {
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
}
