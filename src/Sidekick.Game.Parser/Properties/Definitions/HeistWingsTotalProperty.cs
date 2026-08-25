using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using Sidekick.Game.Providers;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class HeistWingsTotalProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage,
    TradeFilterProvider tradeFilterProvider) : PropertyDefinition
{
    public override string Label => tradeFilterProvider.HeistCategory?.Filters.FirstOrDefault(x => x.Id == "heist_max_wings")?.Text ?? "Total Wings";

    private Regex Pattern { get; } = new($@"^{Regex.Escape(currentGameLanguage.Language.DescriptionHeistWings)}:[^\d]*\d+/(\d+)");

    public override void Parse(Item item)
    {
        item.Properties.HeistWingsTotal = GetInt(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.HeistWingsTotal <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new HeistWingsTotalFilter
        {
            Text = Label,
            Value = item.Properties.HeistWingsTotal,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(HeistWingsTotalProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class HeistWingsTotalFilter : IntPropertyFilter
{
    public HeistWingsTotalFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false, normalizeBy: 0);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;
        query.Filters.GetOrCreateHeistFilters().Filters.WingsTotal = new StatFilterValue(this);
    }
}
