using Sidekick.Apis.Poe2Scout.History.Models;
namespace Sidekick.Apis.Poe2Scout.History;

public interface IScoutHistoryProvider
{
    Task<ScoutHistory?> GetItemHistory(int itemId);
    Task<ScoutHistory?> GetCurrencyHistory(int itemId);
}
