using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class PhysicalDpsProperty(
    GameType game,
    Microsoft.Extensions.Localization.IStringLocalizer<Localization.PoeResources> resources) : PropertyDefinition
{
    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Weapons,
    ];

    public override string Label => resources["PhysicalDps"];

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
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.PhysicalDamage)) ? LineContentType.Augmented : LineContentType.Simple,
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
