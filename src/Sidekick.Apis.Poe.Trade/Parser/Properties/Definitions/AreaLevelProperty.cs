using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class AreaLevelProperty(
    GameType game,
    ISettingsService settingsService,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionAreaLevel.ToRegexIntCapture();

    public override List<ItemClass> ValidItemClasses { get; } = [
        ..ItemClassConstants.Areas,
    ];

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.AreaLevel = GetInt(Pattern, propertyBlock);
        if (item.Properties.AreaLevel > 0) propertyBlock.Parsed = true;
    }

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.AreaLevel <= 0) return null;

        var autoSelectKey = $"Trade_Filter_{nameof(AreaLevelProperty)}_{game.GetValueAttribute()}";
        var filter = new AreaLevelFilter
        {
            Text = gameLanguageProvider.Language.DescriptionAreaLevel,
            NormalizeEnabled = false,
            Value = item.Properties.AreaLevel,
            AutoSelectSettingKey = autoSelectKey,
            AutoSelect = await settingsService.GetObject<AutoSelectPreferences>(autoSelectKey, () => null),
        };
        return filter;
    }
}

public class AreaLevelFilter : IntPropertyFilter
{
    public AreaLevelFilter()
    {
        DefaultAutoSelect = new AutoSelectPreferences()
        {
            Mode = AutoSelectMode.Always,
        };
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.AreaLevel = new StatFilterValue(this);
    }
}
