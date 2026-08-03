using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Items;
using Sidekick.Data.Texts;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RedeemerProperty(
    GameType game,
    DataTextProvider dataTextProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = dataTextProvider.Texts.InfluenceRedeemer.ToRegexLine();

    public override string Label => dataTextProvider.Texts.InfluenceRedeemer;

    public override void Parse(Item item)
    {
        if (item.Properties.Rarity != Rarity.Normal &&
            item.Properties.Rarity != Rarity.Magic &&
            item.Properties.Rarity != Rarity.Rare &&
            item.Properties.Rarity != Rarity.Unique) return;

        item.Properties.Influences.Redeemer = GetBool(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (!item.Properties.Influences.Redeemer) return Task.FromResult<TradeFilter?>(null);

        var filter = new RedeemerFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(RedeemerProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class RedeemerFilter : TradeFilter
{
    public RedeemerFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(true);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMiscFilters().Filters.RedeemerItem = new SearchFilterOption(this);
    }
}
