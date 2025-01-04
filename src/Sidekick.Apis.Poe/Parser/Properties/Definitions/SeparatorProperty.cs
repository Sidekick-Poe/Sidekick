using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class SeparatorProperty() : PropertyDefinition
{
    public const string Text = "---";

    public override List<Category> ValidCategories { get; } = [];

    public override void Initialize()
    {
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue)
    {
        return new BooleanPropertyFilter(this)
        {
            ShowCheckbox = false,
            Text = Text,
            Checked = true,
        };
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
    }
}
