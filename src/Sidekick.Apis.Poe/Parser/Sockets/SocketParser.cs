using System.Text.RegularExpressions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Sockets;

public class SocketParser(IGameLanguageProvider gameLanguageProvider) : ISocketParser
{
    private Regex Pattern { get; } = new Regex($"{Regex.Escape(gameLanguageProvider.Language.DescriptionSockets)}.*?([-RGBWAS]+)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)");

    public List<Socket> Parse(ParsingItem parsingItem)
    {
        if (!parsingItem.TryParseRegex(Pattern, out var match))
        {
            return [];
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
                            Colour = SocketColour.Soulcore
                        });
                        break;
                }

                groupValue = groupValue[1..];
            }
        }

        return result;
    }
}
