using Sidekick.Apis.Poe.Trade.Clients.Models;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Trade.Clients;

public interface ITradeApiClient
{
    string GetDataFileName(GameType game, IGameLanguage language, string path);
    Task<FetchResult<TReturn>> FetchData<TReturn>(GameType game, IGameLanguage language, string path);
    Task<Stream> FetchData(GameType game, IGameLanguage language, string path);
}
