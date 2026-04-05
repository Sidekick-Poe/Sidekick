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

public class HeistRoutesRevealedProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    private readonly ITradeFilterProvider tradeFilterProvider = serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override string Label => tradeFilterProvider.HeistCategory?.Filters.FirstOrDefault(x => x.Id == "heist_escape_routes")?.Text ?? "Escape Routes Revealed";

    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionHeistRoutes.ToRegexIntProperty();

    public override void Parse(Item item)
    {
        item.Properties.HeistRoutesRevealed = GetInt(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.HeistRoutesRevealed <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new HeistRoutesRevealedFilter
        {
            Text = Label,
            Value = item.Properties.HeistRoutesRevealed,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(HeistRoutesRevealedProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class HeistRoutesRevealedFilter : IntPropertyFilter
{
    public HeistRoutesRevealedFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false, normalizeBy: 0);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;
        query.Filters.GetOrCreateHeistFilters().Filters.RoutesRevealed = new StatFilterValue(this);
    }
}
