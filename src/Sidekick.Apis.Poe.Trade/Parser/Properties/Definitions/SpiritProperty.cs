using System.Text.RegularExpressions;
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

public class SpiritProperty(
    GameType game,
    ISettingsService settingsService,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionSpirit.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionSpirit.ToRegexIsAugmented();

    public override List<ItemClass> ValidItemClasses { get; } = [
        ..ItemClassConstants.Weapons,
        ..ItemClassConstants.Equipment,
    ];

    public override string Label => gameLanguageProvider.Language.DescriptionSpirit;

    public override void Parse(Item item)
    {
        if (game == GameType.PathOfExile1) return;
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.Spirit = GetInt(Pattern, propertyBlock);
        if (item.Properties.Spirit == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.Spirit));
    }

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        if (game == GameType.PathOfExile1 || item.Properties.Spirit <= 0) return null;

        var autoSelectKey = $"Trade_Filter_{nameof(SpiritProperty)}_{game.GetValueAttribute()}";
        var filter = new SpiritFilter
        {
            Text = Label,
            NormalizeEnabled = true,
            Value = item.Properties.Spirit,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.Spirit)) ? LineContentType.Augmented : LineContentType.Simple,
            AutoSelectSettingKey = autoSelectKey,
            AutoSelect = await settingsService.GetObject<AutoSelectPreferences>(autoSelectKey, () => null),
        };
        return filter;
    }
}

public class SpiritFilter : IntPropertyFilter
{
    public SpiritFilter()
    {
        DefaultAutoSelect = new AutoSelectPreferences()
        {
            Mode = AutoSelectMode.Never,
        };
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateEquipmentFilters().Filters.Spirit = new StatFilterValue(this);
    }
}
