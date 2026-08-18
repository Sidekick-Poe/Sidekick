using Sidekick.Common.Enums;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Localization;
using Sidekick.Game.Parser.Trade.Requests;
using ItemProperties = Sidekick.Game.Parser.Items.ItemProperties;

namespace Sidekick.Game.Parser.Properties.Definitions;

public class ChaosDpsProperty(
    GameType game,
    Microsoft.Extensions.Localization.IStringLocalizer<PoeResources> resources) : PropertyDefinition
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
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.ChaosDamage)),
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
