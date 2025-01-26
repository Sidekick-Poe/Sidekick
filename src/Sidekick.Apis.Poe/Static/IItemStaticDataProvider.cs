using Sidekick.Apis.Poe.Static.Models;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Static
{
    public interface IItemStaticDataProvider : IInitializableService
    {
        string? GetImage(string id);

        StaticItem? Get(string id);

        StaticItem? Get(ItemHeader itemHeader);
    }
}
