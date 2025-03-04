using Sidekick.Apis.Poe.Clients.Models;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Clients;

public interface IPoeTradeClient
{
    Task<FetchResult<TReturn>> Fetch<TReturn>(GameType game, IGameLanguage language, string path);
}
