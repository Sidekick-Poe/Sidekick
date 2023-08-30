using System.Text.Json;

namespace Sidekick.Apis.Poe.Clients
{
    public interface IPoeTradeClient
    {
        HttpClient HttpClient { get; set; }

        JsonSerializerOptions Options { get; }

        Task<FetchResult<TReturn>> Fetch<TReturn>(string path, bool useDefaultLanguage = false);
    }
}
