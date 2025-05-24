using Sidekick.Apis.Poe.Trade.Clients.Models;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Trade.Clients;

public interface ITradeApiClient
{
    Task<FetchResult<TReturn>> Fetch<TReturn>(GameType game, IGameLanguage language, string path);
}
