using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Parser.Headers;

public interface IHeaderParser : IInitializableService
{
    void Parse(Item item);
}
