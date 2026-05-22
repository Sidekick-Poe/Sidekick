using Sidekick.Data.Stats;
namespace Sidekick.Data.Builder.Stats.Hooks;

public class LogbookFactions(StatBuilderContext context) : BaseHook
{
    public override void OnAfterTradeBuild(List<StatDefinition> definitions)
    {
        var patterns = (from definition in definitions.Where(x => x.TradeStats != null)
            from tradeStat in definition.TradeStats!
            where tradeStat.Category == StatCategory.Pseudo
            where context.InvariantDetails.LogbookFactionStatIds.Contains(tradeStat.Id)
            select definition);

        foreach (var pattern in patterns)
        {
            pattern.Pattern = context.Patterns.GetPattern(pattern.Text.Split(':', 2).Last().Trim());
        }
    }
}
