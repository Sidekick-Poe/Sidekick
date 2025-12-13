using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Clients.Models;

namespace Sidekick.Apis.Poe.Trade.Clients;

public interface ITradeApiClient
{
    Task<FetchResult<TReturn>> FetchData<TReturn>(GameType game, IGameLanguage language, string path);
    Task<Stream> FetchData(GameType game, IGameLanguage language, string path);
}
