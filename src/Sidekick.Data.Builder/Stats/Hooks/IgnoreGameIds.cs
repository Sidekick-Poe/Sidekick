using Sidekick.Data.Stats;
namespace Sidekick.Data.Builder.Stats.Hooks;

public class IgnoreGameIds : BaseHook
{
    private static readonly List<string> IgnoredGameIds =
    [
        "map_set_league_category",
        "map_item_level_override",
    ];

    public override void OnAfterGameBuild(List<StatDefinition> statDefinitions)
    {
        statDefinitions.RemoveAll(x => x.GameIds != null && x.GameIds.Any(y => IgnoredGameIds.Contains(y)));
    }
}
