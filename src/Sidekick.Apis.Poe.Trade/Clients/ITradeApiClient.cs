using Sidekick.Apis.Poe.Trade.Clients.Models;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Clients;

public interface ITradeApiClient
{
    Task<FetchResult<TReturn>> FetchData<TReturn>(GameType game, IGameLanguage language, string path);
    Task<Stream> FetchData(GameType game, IGameLanguage language, string path);
}
