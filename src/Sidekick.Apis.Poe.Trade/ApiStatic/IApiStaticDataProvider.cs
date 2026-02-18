using Sidekick.Common.Initialization;
using Sidekick.Data.Trade.Models;
namespace Sidekick.Apis.Poe.Trade.ApiStatic;

public interface IApiStaticDataProvider : IInitializableService
{
    TradeStaticItem? GetById(string? id);

    TradeStaticItem? Get(string? name, string? type);
}
