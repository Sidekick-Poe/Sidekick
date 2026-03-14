using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Data.Builder.Pseudo.Definitions;
using Sidekick.Data.Builder.Trade;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using Sidekick.Data.Pseudo;
using Sidekick.Data.Trade;
namespace Sidekick.Data.Builder.Pseudo;

public class PseudoBuilder
(
    ILogger<PseudoBuilder> logger,
    IOptions<SidekickConfiguration> configuration,
    DataProvider dataProvider,
    IGameLanguageProvider gameLanguageProvider,
    TradeStatProvider tradeStatProvider
)
{
    public async Task Build(IGameLanguage language)
    {
        try
        {
            await Build(GameType.PathOfExile1, language);
            await Build(GameType.PathOfExile2, language);
        }
        catch (Exception ex)
        {
            if (configuration.Value.ApplicationType == SidekickApplicationType.DataBuilder || configuration.Value.ApplicationType == SidekickApplicationType.Test)
            {
                throw;
            }

            logger.LogError(ex, $"Failed to build pseudo stats for {language.Code}.");
        }
    }

    private async Task Build(GameType game, IGameLanguage language)
    {
        var invariantStats = await tradeStatProvider.GetDefinitions(game, gameLanguageProvider.InvariantLanguage);
        invariantStats.RemoveAll(x => x.Id.StartsWith("pseudo"));

        var localizedStats = await tradeStatProvider.GetDefinitions(game, language);
        localizedStats = localizedStats
            .Where(x => x.Category == StatCategory.Pseudo)
            .ToList();

        List<PseudoDefinitionBuilder> builders = [
            new ElementalResistancesDefinition(),
            new ChaosResistancesDefinition(),
            new StrengthDefinition(),
            new IntelligenceDefinition(),
            new DexterityDefinition(),
            new LifeDefinition(game),
            new ManaDefinition(game),
        ];

        var definitions = GetDefinitions(builders, invariantStats, localizedStats).ToList();
        await dataProvider.Write(game, DataType.Pseudo, language, definitions);
    }

    private IEnumerable<PseudoDefinition> GetDefinitions(List<PseudoDefinitionBuilder> builders, List<TradeStatDefinition> invariantStats, List<TradeStatDefinition> localizedStats)
    {
        foreach (var builder in builders)
        {
            yield return new PseudoDefinition()
            {
                PseudoStatId = builder.StatId,
                Stats = GetStats(builder, invariantStats).ToList(),
                Text = GetText(builder, localizedStats),
            };
        }

    }

    private IEnumerable<PseudoStat> GetStats(PseudoDefinitionBuilder builder, List<TradeStatDefinition> invariantStats)
    {
        foreach (var invariantStat in invariantStats)
        {
            if (builder.Exception != null && builder.Exception.IsMatch(invariantStat.Text)) continue;

            foreach (var pattern in builder.Patterns)
            {
                if (!pattern.Pattern.IsMatch(invariantStat.Text)) continue;

                yield return new PseudoStat(invariantStat.Id, invariantStat.Text, pattern.Multiplier);
            }
        }
    }


    private string? GetText(PseudoDefinitionBuilder builder, List<TradeStatDefinition> localizedStats)
    {
        return localizedStats.FirstOrDefault(x => x.Id == builder.StatId)?.Text ?? null;
    }
}
