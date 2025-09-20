using Sidekick.Apis.Poe.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Parser.Headers;

public interface IHeaderParser : IInitializableService
{
    ItemHeader Parse(ParsingItem parsingItem);
}
