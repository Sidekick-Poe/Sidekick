using Sidekick.Common.Enums;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using Sidekick.Game.Providers;
using ItemProperties = Sidekick.Game.Parser.Items.ItemProperties;

namespace Sidekick.Game.Parser.Properties.Definitions;

public class ElementalDpsProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    public override string Label => gameTextProvider.Texts.TradeElementalDps;

    public override void Parse(Item item) {}

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.ElementalDps <= 0)
        {
            return Task.FromResult<TradeFilter?>(null);
        }

        var filter = new ElementalDpsFilter(game)
        {
            Text = Label,
            Value = item.Properties.ElementalDps ?? 0,
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.FireDamage)) || item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.ColdDamage)) || item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.LightningDamage)),
            AutoSelectSettingKey = $"Trade_Filter_{nameof(ElementalDpsProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };

        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class ElementalDpsFilter : DoublePropertyFilter
{
    public ElementalDpsFilter(GameType game)
    {
        Game = game;
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    private GameType Game { get; }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        switch (Game)
        {
            case GameType.Poe1: query.Filters.GetOrCreateWeaponFilters().Filters.ElementalDps = new StatFilterValue(this); break;
            case GameType.Poe2: query.Filters.GetOrCreateEquipmentFilters().Filters.ElementalDps = new StatFilterValue(this); break;
        }
    }
}
