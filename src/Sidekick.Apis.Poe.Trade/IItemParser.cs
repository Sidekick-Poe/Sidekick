using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade;

public interface IItemParser : IInitializableService
{
    Item ParseItem(string? itemText);
}
