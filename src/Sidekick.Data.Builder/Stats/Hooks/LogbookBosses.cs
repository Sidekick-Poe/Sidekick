using Sidekick.Data.Stats;
namespace Sidekick.Data.Builder.Stats.Hooks;

public class LogbookBosses(StatBuilderContext context) : BaseHook
{
    private const string GameId = "map_expedition_saga_contains_boss";

    public override void OnAfterGameBuild(List<StatDefinition> statDefinitions)
    {
        var tradeDefinitions = context.TradeDefinitions
            .Where(x => context.InvariantDetails.LogbookBossStatIds.Contains(x.Id))
            .ToList();
        if (tradeDefinitions.Count == 0) return;

        foreach (var statDefinition in statDefinitions)
        {
            if (statDefinition.GameIds == null || !statDefinition.GameIds.Contains(GameId)) continue;

            var tradeDefinition = tradeDefinitions.FirstOrDefault(x => x.Id.GetStatOption() == (int?)statDefinition.Value);
            if (tradeDefinition == null) continue;

            statDefinition.TradeIds ??= [];
            statDefinition.TradeIds.Add(tradeDefinition.Id);
        }
    }
}
