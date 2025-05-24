using Sidekick.Apis.Poe.Trade.Static.Models;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Static;

public interface IItemStaticDataProvider : IInitializableService
{
    string? GetImage(string id);

    StaticItem? Get(string id);

    StaticItem? Get(ItemHeader itemHeader);
}
