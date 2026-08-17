using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Providers;
using ItemProperties = Sidekick.Game.Parser.Items.ItemProperties;

namespace Sidekick.Game.Parser.Properties.Definitions;

public class PhysicalDpsProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    public override string Label => gameTextProvider.Texts.TradePhysicalDps;

    public override void Parse(Item item) {}

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.PhysicalDps <= 0)
        {
            return Task.FromResult<TradeFilter?>(null);
        }

        var filter = new PhysicalDpsFilter(game)
        {
            Text = Label,
            Value = item.Properties.PhysicalDpsWithQuality ?? 0,
            OriginalValue = item.Properties.PhysicalDps ?? 0,
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.PhysicalDamage)),
            AutoSelectSettingKey = $"Trade_Filter_{nameof(PhysicalDpsProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };

        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class PhysicalDpsFilter : DoublePropertyFilter
{
    public PhysicalDpsFilter(GameType game)
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
            case GameType.PathOfExile1: query.Filters.GetOrCreateWeaponFilters().Filters.PhysicalDps = new StatFilterValue(this); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.PhysicalDps = new StatFilterValue(this); break;
        }
    }
}
