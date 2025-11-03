using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.PoeNinja.Exchange.Models;
namespace Sidekick.Apis.PoeNinja.Exchange;

public interface INinjaExchangeProvider
{
    Task<NinjaCurrency?> GetCurrencyInfo(string? invariantId);
    Task<ApiOverviewResult> FetchOverview(GameType game, string type);
    Task<Uri?> GetDetailsUri(string? invariantId);
}
