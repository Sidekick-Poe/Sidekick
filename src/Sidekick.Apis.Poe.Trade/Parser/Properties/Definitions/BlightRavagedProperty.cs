using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Game;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Texts;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class BlightRavagedProperty(
    GameType game,
    GameTextProvider dataTextProvider) : PropertyDefinition
{
    public override string Label => dataTextProvider.Texts.ItemBlightRavaged.CleanWildcard();

    public override void Parse(Item item)
    {
        item.Properties.BlightRavaged = item.Definition.Key == "BASEITEM_Metadata/Items/TradeProxy/UberBlightedMap";

        if (item.Properties.BlightRavaged)
        {
            item.TradeItem = null;
            item.InvariantTradeItem = null;
        }
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (!item.Properties.BlightRavaged) return Task.FromResult<TradeFilter?>(null);

        var filter = new BlightRavagedFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(BlightRavagedProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class BlightRavagedFilter : HiddenFilter
{
    public BlightRavagedFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(true);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.BlightRavavaged = new SearchFilterOption(this);
    }
}
