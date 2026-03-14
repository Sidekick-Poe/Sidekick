using Sidekick.Common.Initialization;
using Sidekick.Data.Builder.Trade.Models;
namespace Sidekick.Apis.Poe.Trade.ApiStatic;

public interface IApiStaticDataProvider : IInitializableService
{
    RawTradeStaticItem? GetById(string? id);

    RawTradeStaticItem? Get(string? name, string? type);
}
