using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class SeparatorProperty : PropertyDefinition
{
    public const string Text = "---";

    public override List<ItemClass> ValidItemClasses { get; } = [];

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        return Task.FromResult<TradeFilter?>(new SeparatorFilter
        {
            Text = Text,
            Checked = true,
        });
    }

    public override void PrepareTradeRequest(Query query, Item item, TradeFilter filter)
    {
    }
}
