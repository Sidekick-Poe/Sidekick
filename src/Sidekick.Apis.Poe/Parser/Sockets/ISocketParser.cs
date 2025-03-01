using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Parser.Sockets;

public interface ISocketParser
{
    List<Socket> Parse(ParsingItem parsingItem);
}
