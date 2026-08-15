using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Game;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Providers;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class BlightedProperty(
    GameType game,
    GameTextProvider dataTextProvider) : PropertyDefinition
{
    public override string Label => dataTextProvider.Texts.ItemBlighted.CleanWildcard();

    public override void Parse(Item item)
    {
        item.Properties.Blighted = item.Definition.BaseItemIds?.Contains("Metadata/Items/TradeProxy/BlightedMap") ?? false;

        if (item.Properties.Blighted)
        {
            item.TradeItem = null;
            item.InvariantTradeItem = null;
        }
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (!item.Properties.Blighted) return Task.FromResult<TradeFilter?>(null);

        var filter = new BlightedFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(BlightedProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class BlightedFilter : HiddenFilter
{
    public BlightedFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(true);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.Blighted = new SearchFilterOption(this);
    }
}
