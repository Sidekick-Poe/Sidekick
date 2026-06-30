using Sidekick.Data.Stats;
namespace Sidekick.Data.Builder.Stats.Hooks;

public class IncursionRooms(StatBuilderContext context) : BaseHook
{
    public override void OnAfterTradeBuild(List<StatDefinition> statDefinitions)
    {
        foreach (var statDefinition in statDefinitions)
        {
            if (statDefinition.TradeIds == null) continue;

            foreach (var tradeStat in statDefinition.TradeIds)
            {
                if (!tradeStat.StartsWith("pseudo.")) continue;
                if (!context.InvariantDetails.IncursionRoomStatIds.Contains(tradeStat)) continue;

                statDefinition.Pattern = context.PatternBuilder.GetPattern(statDefinition.Text.Split(':', 2).Last().Trim());
            }
        }
    }
}
