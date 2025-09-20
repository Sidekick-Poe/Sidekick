using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class SocketProperty
(
    IGameLanguageProvider gameLanguageProvider,
    GameType game,
    IStringLocalizer<PoeResources> resources
) : PropertyDefinition
{
    private Regex Pattern { get; } = new Regex($"{Regex.Escape(gameLanguageProvider.Language.DescriptionSockets)}.*?([-RGBWAS]+)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)");

    public override List<Category> ValidCategories { get; } = [Category.Armour, Category.Weapon, Category.Accessory, Category.Gem];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem, ItemHeader header)
    {
        if (!parsingItem.TryParseRegex(Pattern, out var match))
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

        itemProperties.Sockets = result;
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.Sockets is not
            {
                Count: > 0
            })
            return null;

        int value;
        bool @checked;
        string? hint;
        if (game == GameType.PathOfExile2)
        {
            // In Path of Exile 2, we automatically check the socket filter when there are 3 or more sockets on the item.
            // 3 sockets on equipment means it was the result of a corrupted outcome. Gems also have sockets on them. The higher the count, the better. We want to price accordingly.
            value = item.Properties.Sockets.Count;
            @checked = value >= 3;
            hint = null;
        }
        else
        {
            // In Path of Exile 1, we automatically check the socket filter when there are 5 or 6 links.
            // The socket filter is actually a link filter in PoE1 as that is what holds value in the game.
            value = item.Properties.Sockets.GroupBy(x => x.Group).Select(x => x.Count()).Max();
            @checked = value >= 5;
            hint = resources["Socket_Poe1_Hint"];
        }

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionSockets,
            NormalizeEnabled = false,
            NormalizeValue = normalizeValue,
            Value = value,
            Checked = @checked,
            Hint = hint,
        };
        filter.ChangeFilterType(filterType);
        return filter;
    }

    public override void PrepareTradeRequest(Query query, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        switch (game)
        {
            case GameType.PathOfExile: query.Filters.GetOrCreateSocketFilters().Filters.Links = new SocketFilterOption(intFilter); break;

            case GameType.PathOfExile2:
                switch (item.Header.Category)
                {
                    case Category.Gem: query.Filters.GetOrCreateMiscFilters().Filters.GemSockets = new StatFilterValue(intFilter); break;
                    default: query.Filters.GetOrCreateEquipmentFilters().Filters.RuneSockets = new StatFilterValue(intFilter); break;
                }

                break;
        }
    }
}
