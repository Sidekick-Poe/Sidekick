using Sidekick.Data.Stats;
namespace Sidekick.Data.Builder.Stats.Hooks;

public abstract class BaseHook
{
    public virtual void OnAfterGameBuild(List<StatDefinition> definitions) {}
    public virtual void OnAfterTradeBuild(List<StatDefinition> definitions) {}
}
