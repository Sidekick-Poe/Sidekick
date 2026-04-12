using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ShaperProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.InfluenceShaper.ToRegexLine();

    public override string Label => currentGameLanguage.Language.InfluenceShaper;

    public override void Parse(Item item)
    {
        if (!item.ItemClass.IsEquipment() &&
            !item.ItemClass.IsWeapon() &&
            !item.ItemClass.IsAccessory()) return;

        item.Properties.Influences.Shaper = GetBool(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (!item.Properties.Influences.Shaper) return Task.FromResult<TradeFilter?>(null);

        var filter = new ShaperFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(ShaperProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class ShaperFilter : TradeFilter
{
    public ShaperFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(true);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMiscFilters().Filters.ShaperItem = new SearchFilterOption(this);
    }
}
