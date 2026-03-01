using Sidekick.Common.Initialization;
using Sidekick.Data.Trade.Raw;
namespace Sidekick.Apis.Poe.Trade.ApiStatic;

public interface IApiStaticDataProvider : IInitializableService
{
    RawTradeStaticItem? GetById(string? id);

    RawTradeStaticItem? Get(string? name, string? type);
}
