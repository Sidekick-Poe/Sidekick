using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Results;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Items;
using ItemProperties = Sidekick.Data.Items.ItemProperties;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ChaosDpsProperty(
    GameType game,
    Microsoft.Extensions.Localization.IStringLocalizer<Localization.PoeResources> resources) : PropertyDefinition
{
    public override string Label => resources["ChaosDps"];

    public override void Parse(Item item) {}

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.ChaosDps <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new ChaosDpsFilter
        {
            Text = Label,
            Value = item.Properties.ChaosDps ?? 0,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.ChaosDamage)) ? LineContentType.Augmented : LineContentType.Simple,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(ChaosDpsProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };

        return Task.FromResult<TradeFilter?>(filter);
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
