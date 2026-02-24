using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class HunterProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.InfluenceHunter.ToRegexLine();

    public override string Label => currentGameLanguage.Language.InfluenceHunter;

    public override void Parse(Item item)
    {
        if (!ItemClassConstants.Equipment.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Accessories.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Weapons.Contains(item.Properties.ItemClass)) return;

        item.Properties.Influences.Hunter = GetBool(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (!item.Properties.Influences.Hunter) return Task.FromResult<TradeFilter?>(null);

        var filter = new HunterFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(HunterProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class HunterFilter : TradeFilter
{
    public HunterFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(true);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMiscFilters().Filters.HunterItem = new SearchFilterOption(this);
    }
}
