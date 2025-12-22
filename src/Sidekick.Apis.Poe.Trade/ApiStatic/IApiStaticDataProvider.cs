using Sidekick.Apis.Poe.Trade.ApiStatic.Models;
using Sidekick.Common.Initialization;
namespace Sidekick.Apis.Poe.Trade.ApiStatic;

public interface IApiStaticDataProvider : IInitializableService
{
    StaticItem? GetById(string? id);

    StaticItem? Get(string? name, string? type);
}
