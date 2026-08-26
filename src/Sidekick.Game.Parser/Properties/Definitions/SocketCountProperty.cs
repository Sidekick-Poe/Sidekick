using System.Text.RegularExpressions;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class SocketCountProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = new Regex($"^{Regex.Escape(currentGameLanguage.Language.DescriptionSockets)}.*?([-RGBWAS]+)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)");

    public override string Label => currentGameLanguage.Language.DescriptionSockets;

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

        TradeFilter? filter = null;
        switch (game)
        {
            case GameType.Poe1:
            {
                filter = new Poe1SocketCountFilter()
                {
                    Text = Label,
                    Value = item.Properties.Sockets.Count,
                    AutoSelectSettingKey = $"Trade_Filter_{nameof(SocketCountProperty)}_{game.GetValueAttribute()}",
                    NormalizeEnabled = false,
                };
                break;
            }

            case GameType.Poe2:
            {
                filter = new Poe2SocketCountFilter()
                {
                    Text = Label,
                    Value = item.Properties.Sockets.Count,
                    AutoSelectSettingKey = $"Trade_Filter_{nameof(SocketCountProperty)}_{game.GetValueAttribute()}",
                    NormalizeEnabled = false,
                };
                break;
            }
        }

        return Task.FromResult(filter);
    }
}

public class Poe1SocketCountFilter : SocketPropertyFilter
{
    public Poe1SocketCountFilter()
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
                            Value = 6.ToString(),
                        },
                    ],
                },
            ],
        };
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateSocketFilters().Filters.Sockets = new SocketFilterOption(this);
    }
}

public class Poe2SocketCountFilter : IntPropertyFilter
{
    public Poe2SocketCountFilter()
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

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        switch (item.Properties.Rarity)
        {
            case Rarity.Gem: query.Filters.GetOrCreateMiscFilters().Filters.GemSockets = new StatFilterValue(this); break;
            default: query.Filters.GetOrCreateEquipmentFilters().Filters.RuneSockets = new StatFilterValue(this); break;
        }
    }
}
