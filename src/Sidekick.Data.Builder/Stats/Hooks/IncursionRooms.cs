using Sidekick.Data.Stats;
namespace Sidekick.Data.Builder.Stats.Hooks;

public class IncursionRooms(StatBuilderContext context) : BaseHook
{
    public override void OnAfterTradeBuild(List<StatDefinition> definitions)
    {
        var patterns = (from definition in definitions.Where(x => x.TradeStats != null)
            from tradeStat in definition.TradeStats!
            where tradeStat.Category == StatCategory.Pseudo
            where context.InvariantDetails.IncursionRoomStatIds.Contains(tradeStat.Id)
            select definition);

        foreach (var pattern in patterns)
        {
            pattern.Pattern = context.Patterns.GetPattern(pattern.Text.Split(':', 2).Last().Trim());
        }

        definitions.RemoveAll(x => x.TradeStats != null && x.TradeStats.Any(y => context.InvariantDetails.IncursionRoomStatIds.Contains(y.Id)) && Predicate(x));

        return;

        bool Predicate(StatDefinition x) => x.TradeStats != null && x.TradeStats.All(y => y.Option?.Id == 2);
    }
}
