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

public class HunterProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.InfluenceHunter.ToRegexLine();

    public override string Label => currentGameLanguage.Language.InfluenceHunter;

    public override void Parse(Item item)
    {
        if (!item.Definition.ItemClass.IsEquipment() &&
            !item.Definition.ItemClass.IsWeapon() &&
            !item.Definition.ItemClass.IsAccessory()) return;

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
