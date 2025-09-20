using Sidekick.Apis.Poe.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Parser.Headers;

public interface IItemClassParser : IInitializableService
{
    ItemClass Parse(ParsingItem parsingItem);
}
