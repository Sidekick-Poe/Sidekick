using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Items;
using Sidekick.Data.Stats;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ImbuedGemProperty(
    GameType game,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    private readonly ITradeFilterProvider tradeFilterProvider = serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override string Label => tradeFilterProvider.Imbued?.Text ?? "Imbued";

    public override void ParseAfterStats(Item item)
    {
        if (game != GameType.PathOfExile1) return;
        if (item.ItemClass != ItemClass.ActiveSkillGem) return;

        item.Properties.Imbued = item.Stats.Any(x => x.Category == StatCategory.Imbued);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game != GameType.PathOfExile1) return Task.FromResult<TradeFilter?>(null);
        if (item.ItemClass != ItemClass.ActiveSkillGem) return Task.FromResult<TradeFilter?>(null);

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
