using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ChaosDpsProperty(Microsoft.Extensions.Localization.IStringLocalizer<Sidekick.Apis.Poe.Trade.Localization.PoeResources> resources) : PropertyDefinition
{
    public override List<ItemClass> ValidItemClasses { get; } = [
        ..ItemClassConstants.Weapons,
    ];

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.ChaosDps <= 0)
        {
            return Task.FromResult<TradeFilter?>(null);
        }

        var filter = new ChaosDpsFilter
        {
            Text = resources["ChaosDps"],
            NormalizeEnabled = true,
            Value = item.Properties.ChaosDps ?? 0,
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.ChaosDamage)) ? LineContentType.Augmented : LineContentType.Simple,
        };

        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class ChaosDpsFilter : DoublePropertyFilter
{
    public override void PrepareTradeRequest(Query query, Item item)
    {
    }
}
