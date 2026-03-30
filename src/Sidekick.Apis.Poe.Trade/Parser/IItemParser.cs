using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Initialization;
using Sidekick.Data.Items;
namespace Sidekick.Apis.Poe.Trade.Parser;

public interface IItemParser : IInitializableService
{
    Item ParseItem(string? text);
}
