using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Providers;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class CrusaderProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameTextProvider.Texts.InfluenceCrusader.ToRegexLine();

    public override string Label => gameTextProvider.Texts.InfluenceCrusader;

    public override void Parse(Item item)
    {
        if (item.Properties.Rarity != Rarity.Normal &&
            item.Properties.Rarity != Rarity.Magic &&
            item.Properties.Rarity != Rarity.Rare &&
            item.Properties.Rarity != Rarity.Unique) return;

        if (item.Properties.Rarity == Rarity.DivinationCard) return;
        item.Properties.Influences.Crusader = GetBool(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (!item.Properties.Influences.Crusader) return Task.FromResult<TradeFilter?>(null);

        var filter = new CrusaderFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(CrusaderProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class CrusaderFilter : TradeFilter
{
    public CrusaderFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(true);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMiscFilters().Filters.CrusaderItem = new SearchFilterOption(this);
    }
}
