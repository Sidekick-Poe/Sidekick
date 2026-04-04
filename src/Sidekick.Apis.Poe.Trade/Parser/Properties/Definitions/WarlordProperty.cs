using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class WarlordProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.InfluenceWarlord.ToRegexLine();

    public override string Label => currentGameLanguage.Language.InfluenceWarlord;

    public override void Parse(Item item)
    {
        if (!item.Definition.ItemClass.IsEquipment() &&
            !item.Definition.ItemClass.IsWeapon() &&
            !item.Definition.ItemClass.IsAccessory()) return;

        item.Properties.Influences.Warlord = GetBool(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (!item.Properties.Influences.Warlord) return Task.FromResult<TradeFilter?>(null);

        var filter = new WarlordFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(WarlordProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class WarlordFilter : TradeFilter
{
    public WarlordFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(true);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMiscFilters().Filters.WarlordItem = new SearchFilterOption(this);
    }
}
