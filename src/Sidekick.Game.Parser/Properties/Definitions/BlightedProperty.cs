using System.Text.RegularExpressions;
using Sidekick.Game.Enums;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using Sidekick.Game.Providers;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class BlightedProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameTextProvider.Texts.ItemBlighted.ToRegexStringProperty();

    public override string Label => gameTextProvider.Texts.ItemBlighted.CleanWildcard();

    public override void Parse(Item item)
    {
        item.Properties.Blighted = GetBool(Pattern, item.Text.Blocks[0]);
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
