using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class PhysicalDpsProperty(GameType game, Microsoft.Extensions.Localization.IStringLocalizer<Sidekick.Apis.Poe.Trade.Localization.PoeResources> resources) : PropertyDefinition
{
    public override List<ItemClass> ValidItemClasses { get; } = [
        ..ItemClassConstants.Weapons,
    ];

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.PhysicalDps <= 0)
        {
            return Task.FromResult<TradeFilter?>(null);
        }

        var filter = new PhysicalDpsFilter(game)
        {
            Text = resources["PhysicalDps"],
            NormalizeEnabled = true,
            Value = item.Properties.PhysicalDpsWithQuality ?? 0,
            OriginalValue = item.Properties.PhysicalDps ?? 0,
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.PhysicalDamage)) ? LineContentType.Augmented : LineContentType.Simple,
        };

        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class PhysicalDpsFilter(GameType game) : DoublePropertyFilter
{
    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        switch (game)
        {
            case GameType.PathOfExile1: query.Filters.GetOrCreateWeaponFilters().Filters.PhysicalDps = new StatFilterValue(this); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.PhysicalDps = new StatFilterValue(this); break;
        }
    }
}
