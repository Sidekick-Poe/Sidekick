using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Game.Parser.Items;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class SocketLinkProperty(
    GameType game,
    IStringLocalizer<PoeResources> resources) : PropertyDefinition
{
    public override string Label => resources["Sockets_Links"];

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Game == GameType.PathOfExile2) return Task.FromResult<TradeFilter?>(null);

        if (item.Properties.Sockets is not
            {
                Count: > 0
            })
            return Task.FromResult<TradeFilter?>(null);

        var filter = new SocketLinkFilter(game)
        {
            Text = Label,
            Value = item.Properties.Sockets.GroupBy(x => x.Group).Select(x => x.Count()).Max(),
            AutoSelectSettingKey = $"Trade_Filter_{nameof(SocketLinkProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class SocketLinkFilter : IntPropertyFilter
{
    public SocketLinkFilter(GameType game)
    {
        Game = game;
        if (game == GameType.PathOfExile1)
        {
            DefaultAutoSelect = new AutoSelectPreferences()
            {
                Mode = AutoSelectMode.Default,
                Rules =
                [
                    new()
                    {
                        Checked = true,
                        NormalizeBy = 0,
                        FillMinRange = true,
                        FillMaxRange = false,
                        Conditions =
                        [
                            new()
                            {
                                Type = AutoSelectConditionType.SocketLink,
                                Comparison = AutoSelectComparisonType.GreaterThanOrEqual,
                                Value = 5.ToString(),
                            },
                        ],
                    },
                ],
            };
        }
    }

    private GameType Game { get; }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        switch (Game)
        {
            case GameType.PathOfExile1: query.Filters.GetOrCreateSocketFilters().Filters.Links = new SocketFilterOption(this); break;
        }
    }
}
