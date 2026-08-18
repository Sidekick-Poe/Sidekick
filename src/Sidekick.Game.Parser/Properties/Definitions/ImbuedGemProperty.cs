using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Common.Enums;
using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Stats;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class ImbuedGemProperty(
    GameType game,
    TradeFilterProvider tradeFilterProvider) : PropertyDefinition
{
    public override string Label => tradeFilterProvider.Imbued?.Text ?? "Imbued";

    public override void ParseAfterStats(Item item)
    {
        if (game != GameType.PathOfExile1) return;
        if (item.ItemClass.Type != ItemClass.ActiveSkillGem) return;

        item.Properties.Imbued = item.Stats.Any(x => x.Category == StatCategory.Imbued);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game != GameType.PathOfExile1) return Task.FromResult<TradeFilter?>(null);
        if (item.ItemClass.Type != ItemClass.ActiveSkillGem) return Task.FromResult<TradeFilter?>(null);

        var filter = new ImbuedGemFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(ImbuedGemProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class ImbuedGemFilter : TriStatePropertyFilter
{
    public ImbuedGemFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(null);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (Checked == null) return;

        query.Filters.GetOrCreateMiscFilters().Filters.Imbued = new SearchFilterOption(this);
    }
}
