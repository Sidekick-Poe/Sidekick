using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class SeparatorProperty() : PropertyDefinition
{
    public const string Text = "---";

    public override List<Category> ValidItemClasses { get; } = [];

    public override Task<PropertyFilter?> GetFilter(Item item)
    {
        return Task.FromResult<PropertyFilter?>(new PropertyFilter(this)
        {
            Text = Text,
            Checked = true,
        });
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
    }
}
