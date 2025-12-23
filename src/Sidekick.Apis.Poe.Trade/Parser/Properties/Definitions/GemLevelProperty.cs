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

public class GemLevelProperty(
    GameType game,
    ISettingsService settingsService,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionLevel.ToRegexIntCapture();

    public override List<ItemClass> ValidItemClasses { get; } = [..ItemClassConstants.Gems];

    public override void Parse(Item item)
    {
        if (item.Properties.ItemClass is ItemClass.UncutSkillGem or ItemClass.UncutSupportGem or ItemClass.UncutSpiritGem)
        {
            return;
        }

        var propertyBlock = item.Text.Blocks[1];
        item.Properties.GemLevel = GetInt(Pattern, propertyBlock);

        if (item.Properties.GemLevel > 0) propertyBlock.Parsed = true;
    }

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.GemLevel <= 0) return null;

        var autoSelectKey = $"Trade_Filter_{nameof(GemLevelProperty)}_{game.GetValueAttribute()}";
        var filter = new GemLevelFilter
        {
            Text = gameLanguageProvider.Language.DescriptionLevel,
            NormalizeEnabled = false,
            Value = item.Properties.GemLevel,
            AutoSelectSettingKey = autoSelectKey,
            AutoSelect = await settingsService.GetObject<AutoSelectPreferences>(autoSelectKey, () => null),
        };
        return filter;
    }
}

public class GemLevelFilter : IntPropertyFilter
{
    public GemLevelFilter()
    {
        DefaultAutoSelect = new AutoSelectPreferences()
        {
            Mode = AutoSelectMode.Always,
        };
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMiscFilters().Filters.GemLevel = new StatFilterValue(this);
    }
}
