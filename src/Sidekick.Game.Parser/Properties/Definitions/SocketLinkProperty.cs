using Microsoft.Extensions.Localization;
using Sidekick.Common.Enums;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Localization;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class SocketLinkProperty(
    GameType game,
    IStringLocalizer<PoeResources> resources) : PropertyDefinition
{
    public override string Label => resources["Socket_Links"];

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Game == GameType.Poe2) return Task.FromResult<TradeFilter?>(null);

        if (item.Properties.Sockets is not
            {
                Count: > 0
            })
            return Task.FromResult<TradeFilter?>(null);

        var filter = new SocketLinkFilter()
        {
            Text = Label,
            Value = item.Properties.Sockets.GroupBy(x => x.Group).Select(x => x.Count()).Max(),
            AutoSelectSettingKey = $"Trade_Filter_{nameof(SocketLinkProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class SocketLinkFilter : SocketPropertyFilter
{
    public SocketLinkFilter()
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

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateSocketFilters().Filters.Links = new SocketFilterOption(this);
    }
}
