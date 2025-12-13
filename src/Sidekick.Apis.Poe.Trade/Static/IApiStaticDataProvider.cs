using Sidekick.Apis.Poe.Trade.Static.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Static;

public interface IApiStaticDataProvider : IInitializableService
{
    StaticItem? GetById(string? id);

    StaticItem? Get(string? name, string? type);
}
