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

public class HeistRoomsTotalProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    private readonly ITradeFilterProvider tradeFilterProvider = serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override string Label => tradeFilterProvider.HeistCategory?.Filters.FirstOrDefault(x => x.Id == "heist_max_reward_rooms")?.Text ?? "Total Reward Rooms";

    private Regex Pattern { get; } = new($@"^{Regex.Escape(currentGameLanguage.Language.DescriptionHeistRooms)}:[^\d]*\d+/(\d+)");

    public override void Parse(Item item)
    {
        item.Properties.HeistRoomsTotal = GetInt(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.HeistRoomsTotal <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new HeistRoomsTotalFilter
        {
            Text = Label,
            Value = item.Properties.HeistRoomsTotal,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(HeistRoomsTotalProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class HeistRoomsTotalFilter : IntPropertyFilter
{
    public HeistRoomsTotalFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false, normalizeBy: 0);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;
        query.Filters.GetOrCreateHeistFilters().Filters.RoomsTotal = new StatFilterValue(this);
    }
}
