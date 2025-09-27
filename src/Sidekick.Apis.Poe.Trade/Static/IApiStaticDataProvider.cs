using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Static.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Static;

public interface IApiStaticDataProvider : IInitializableService
{
    string? GetImage(string id);

    StaticItem? GetById(string id);

    StaticItem? Get(string? name, string? type);
}
