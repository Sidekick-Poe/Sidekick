using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class PhysicalDpsProperty(
    GameType game,
    ISettingsService settingsService,
    IGameLanguageProvider gameLanguageProvider,
    Microsoft.Extensions.Localization.IStringLocalizer<Localization.PoeResources> resources) : PropertyDefinition
{
    public override List<ItemClass> ValidItemClasses { get; } = [
        ..ItemClassConstants.Weapons,
    ];

    public override string Label => resources["PhysicalDps"];

    public override void Parse(Item item)
    {
    }

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.PhysicalDps <= 0)
        {
            return null;
        }

        var autoSelectKey = $"Trade_Filter_{nameof(PhysicalDpsProperty)}_{game.GetValueAttribute()}";
        var filter = new PhysicalDpsFilter(game)
        {
            Text = Label,
            NormalizeEnabled = true,
            Value = item.Properties.PhysicalDpsWithQuality ?? 0,
            OriginalValue = item.Properties.PhysicalDps ?? 0,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.PhysicalDamage)) ? LineContentType.Augmented : LineContentType.Simple,
            AutoSelectSettingKey = autoSelectKey,
            AutoSelect = await settingsService.GetObject<AutoSelectPreferences>(autoSelectKey, () => null),
        };

        return filter;
    }
}

public class PhysicalDpsFilter : DoublePropertyFilter
{
    public PhysicalDpsFilter(GameType game)
    {
        Game = game;
        DefaultAutoSelect = new AutoSelectPreferences()
        {
            Mode = AutoSelectMode.Never,
        };
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
