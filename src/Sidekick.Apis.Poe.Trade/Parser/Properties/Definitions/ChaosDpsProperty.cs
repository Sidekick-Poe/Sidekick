using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ChaosDpsProperty(
    GameType game,
    Microsoft.Extensions.Localization.IStringLocalizer<Localization.PoeResources> resources) : PropertyDefinition
{
    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Weapons,
    ];

    public override string Label => resources["ChaosDps"];

    public override void Parse(Item item) {}

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.ChaosDps <= 0) return null;

        var filter = new ChaosDpsFilter
        {
            Text = Label,
            Value = item.Properties.ChaosDps ?? 0,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.ChaosDamage)) ? LineContentType.Augmented : LineContentType.Simple,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(ChaosDpsProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };

        return filter;
    }
}

public class ChaosDpsFilter : DoublePropertyFilter
{
    public ChaosDpsFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item) {}
}
