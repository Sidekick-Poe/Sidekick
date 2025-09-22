using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade;

public interface IItemParser : IInitializableService
{
    Item ParseItem(string? text, string? advancedText = null);
}
