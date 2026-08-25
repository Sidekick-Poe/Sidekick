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

public class HeistRoomsRevealedProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage,
    TradeFilterProvider tradeFilterProvider) : PropertyDefinition
{
    public override string Label => tradeFilterProvider.HeistCategory?.Filters.FirstOrDefault(x => x.Id == "heist_reward_rooms")?.Text ?? "Reward Rooms Revealed";

    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionHeistRooms.ToRegexIntProperty();

    public override void Parse(Item item)
    {
        item.Properties.HeistRoomsRevealed = GetInt(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.HeistRoomsRevealed <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new HeistRoomsRevealedFilter
        {
            Text = Label,
            Value = item.Properties.HeistRoomsRevealed,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(HeistRoomsRevealedProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class HeistRoomsRevealedFilter : IntPropertyFilter
{
    public HeistRoomsRevealedFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false, normalizeBy: 0);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;
        query.Filters.GetOrCreateHeistFilters().Filters.RoomsRevealed = new StatFilterValue(this);
    }
}
