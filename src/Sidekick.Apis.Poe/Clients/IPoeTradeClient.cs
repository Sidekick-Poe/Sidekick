using System.Text.Json;
using Sidekick.Apis.Poe.Clients.Models;
using Sidekick.Common.Game;

namespace Sidekick.Apis.Poe.Clients;

public interface IPoeTradeClient
{
    HttpClient HttpClient { get; }

    JsonSerializerOptions Options { get; }

    Task<FetchResult<TReturn>> Fetch<TReturn>(GameType game, string path, bool useDefaultLanguage = false);
}
