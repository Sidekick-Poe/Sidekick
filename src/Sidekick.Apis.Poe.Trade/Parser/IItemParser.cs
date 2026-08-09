using Sidekick.Common.Initialization;
using Sidekick.Game.Items;
namespace Sidekick.Apis.Poe.Trade.Parser;

public interface IItemParser : IInitializableService
{
    Item ParseItem(string? text);
}
