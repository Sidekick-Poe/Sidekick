using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Parser.Sockets;

public interface ISocketParser: IInitializableService
{
    List<Socket> Parse(ParsingItem parsingItem);
}
