using Sidekick.Apis.Poe.Trade.Trade.Results;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Properties.Definitions;
namespace Sidekick.Modules.Items.Trade.Filters.Types;

public static class FilterExtensions
{
    public static LineContentType GetLineContentType(this TradeFilter filter)
    {
        if (filter is RewardFilter) return LineContentType.Unique;
        if (filter.Augmented) return LineContentType.Augmented;
        return LineContentType.Simple;
    }
}
