using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class SeparatorProperty() : PropertyDefinition
{
    public const string Text = "---";

    public override List<Category> ValidCategories { get; } = [];

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        return new BooleanPropertyFilter(this)
        {
            Text = Text,
            Checked = true,
        };
    }

    public override void PrepareTradeRequest(Query query, Item item, BooleanPropertyFilter filter)
    {
    }
}
