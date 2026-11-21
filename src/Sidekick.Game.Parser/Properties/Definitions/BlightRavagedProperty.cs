using System.Text.RegularExpressions;
using Sidekick.Game.Enums;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using Sidekick.Game.Providers;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class BlightRavagedProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameTextProvider.Texts.ItemBlightRavaged.ToRegexStringProperty();

    public override string Label => gameTextProvider.Texts.ItemBlightRavaged.CleanWildcard();

    public override void Parse(Item item)
    {
        item.Properties.BlightRavaged = GetBool(Pattern, item.Text.Blocks[0]);
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
