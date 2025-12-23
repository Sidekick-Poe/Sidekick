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

public class RedeemerProperty(
    GameType game,
    ISettingsService settingsService,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.InfluenceRedeemer.ToRegexLine();

    public override List<ItemClass> ValidItemClasses { get; } = [
        ..ItemClassConstants.Equipment,
        ..ItemClassConstants.Accessories,
        ..ItemClassConstants.Weapons,
    ];

    public override void Parse(Item item)
    {
        item.Properties.Influences.Redeemer = GetBool(Pattern, item.Text);
    }

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        if (!item.Properties.Influences.Redeemer) return null;

        var autoSelectKey = $"Trade_Filter_{nameof(RedeemerProperty)}_{game.GetValueAttribute()}";
        var filter = new RedeemerFilter
        {
            Text = gameLanguageProvider.Language.InfluenceRedeemer,
            AutoSelectSettingKey = autoSelectKey,
            AutoSelect = await settingsService.GetObject<AutoSelectPreferences>(autoSelectKey, () => null),
        };
        return filter;
    }
}

public class RedeemerFilter : TradeFilter
{
    public RedeemerFilter()
    {
        DefaultAutoSelect = new AutoSelectPreferences()
        {
            Mode = AutoSelectMode.Always,
        };
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMiscFilters().Filters.RedeemerItem = new SearchFilterOption(this);
    }
}
