using Sidekick.Data.Stats;
namespace Sidekick.Data.Builder.Stats.Hooks;

public class LogbookBosses(StatBuilderContext context) : BaseHook
{
    private const string GameId = "map_expedition_saga_contains_boss";

    public override void OnAfterGameBuild(List<StatDefinition> definitions)
    {
        var tradeDefinitions = context.TradeDefinitions.Where(x => x.Id == context.InvariantDetails.LogbookBossesStatId).ToList();
        if (tradeDefinitions.Count == 0) return;

        foreach (var definition in definitions)
        {
            if (definition.GameIds == null) continue;
            if (!definition.GameIds.Contains(GameId)) continue;

            foreach (var tradeDefinition in tradeDefinitions)
            {
                if (tradeDefinition.Option == null) continue;
                if (!definition.Text.EndsWith(tradeDefinition.Option.Text)) continue;
                definition.TradeStats ??= [];
                definition.TradeStats.Add(tradeDefinition);
            }
        }
    }
}
