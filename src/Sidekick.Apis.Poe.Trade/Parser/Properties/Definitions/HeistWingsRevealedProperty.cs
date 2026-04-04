using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class HeistWingsRevealedProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    private readonly ITradeFilterProvider tradeFilterProvider = serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override string Label => tradeFilterProvider.HeistCategory?.Filters.FirstOrDefault(x => x.Id == "heist_wings")?.Text ?? "Wings Revealed";

    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionHeistWings.ToRegexIntProperty();

    public override void Parse(Item item)
    {
        var block = item.Text.Blocks[1];
        item.Properties.HeistWingsRevealed = GetInt(Pattern, block);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.HeistWingsRevealed <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new HeistWingsRevealedFilter
        {
            Text = Label,
            Value = item.Properties.HeistWingsRevealed,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(HeistWingsRevealedProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class HeistWingsRevealedFilter : IntPropertyFilter
{
    public HeistWingsRevealedFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false, normalizeBy: 0);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;
        query.Filters.GetOrCreateHeistFilters().Filters.WingsRevealed = new StatFilterValue(this);
    }
}
