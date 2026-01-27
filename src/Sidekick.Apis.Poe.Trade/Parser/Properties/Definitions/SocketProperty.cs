using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class SocketProperty(
    GameType game,
    IGameLanguageProvider gameLanguageProvider,
    IStringLocalizer<PoeResources> resources) : PropertyDefinition
{
    private Regex Pattern { get; } = new Regex($"{Regex.Escape(gameLanguageProvider.Language.DescriptionSockets)}.*?([-RGBWAS]+)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)");

    public override string Label => gameLanguageProvider.Language.DescriptionSockets;

    public override void Parse(Item item)
    {
        if (!item.Text.TryParseRegex(Pattern, out var match))
        {
            return;
        }

        var groups = match.Groups.Values.Where(x => !string.IsNullOrEmpty(x.Value))
            .Skip(1)
            .Select((x, index) => new
            {
                x.Value,
                Index = index,
            })
            .ToList();

        var result = new List<Socket>();

        foreach (var group in groups)
        {
            var groupValue = group.Value.Replace("-", "").Trim();
            while (groupValue.Length > 0)
            {
                switch (groupValue[0])
                {
                    case 'B':
                        result.Add(new Socket()
                        {
                            Group = group.Index,
                            Colour = SocketColour.Blue
                        });
                        break;

                    case 'G':
                        result.Add(new Socket()
                        {
                            Group = group.Index,
                            Colour = SocketColour.Green
                        });
                        break;

                    case 'R':
                        result.Add(new Socket()
                        {
                            Group = group.Index,
                            Colour = SocketColour.Red
                        });
                        break;

                    case 'W':
                        result.Add(new Socket()
                        {
                            Group = group.Index,
                            Colour = SocketColour.White
                        });
                        break;

                    case 'A':
                        result.Add(new Socket()
                        {
                            Group = group.Index,
                            Colour = SocketColour.Abyss
                        });
                        break;

                    case 'S':
                        result.Add(new Socket()
                        {
                            Group = group.Index,
                            Colour = SocketColour.PoE2
                        });
                        break;
                }

                groupValue = groupValue[1..];
            }
        }

        item.Properties.Sockets = result;
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.Sockets is not
            {
                Count: > 0
            })
            return Task.FromResult<TradeFilter?>(null);

        int value;
        string? hint;
        if (game == GameType.PathOfExile2)
        {
            value = item.Properties.Sockets.Count;
            hint = null;
        }
        else
        {
            value = item.Properties.Sockets.GroupBy(x => x.Group).Select(x => x.Count()).Max();
            hint = resources["Socket_Poe1_Hint"];
        }

        var filter = new SocketFilter(game)
        {
            Text = Label,
            Value = value,
            Hint = hint,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(SocketProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class SocketFilter : IntPropertyFilter
{
    public SocketFilter(GameType game)
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
                                Type = AutoSelectConditionType.SocketCount,
                                Comparison = AutoSelectComparisonType.GreaterThanOrEqual,
                                Value = 5.ToString(),
                            },
                        ],
                    },
                ],
            };
        }
        else
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
                                Type = AutoSelectConditionType.SocketCount,
                                Comparison = AutoSelectComparisonType.GreaterThanOrEqual,
                                Value = 3.ToString(),
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

            case GameType.PathOfExile2:
                switch (item.Properties.ItemClass)
                {
                    case ItemClass.ActiveGem: query.Filters.GetOrCreateMiscFilters().Filters.GemSockets = new StatFilterValue(this); break;
                    default: query.Filters.GetOrCreateEquipmentFilters().Filters.RuneSockets = new StatFilterValue(this); break;
                }

                break;
        }
    }
}
