using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class SeparatorProperty : PropertyDefinition
{
    public const string Text = "---";

    public override List<ItemClass> ValidItemClasses { get; } = [];

    public override string Label => Text;

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        return Task.FromResult<TradeFilter?>(new SeparatorFilter
        {
            Text = Label,
        });
    }
}
