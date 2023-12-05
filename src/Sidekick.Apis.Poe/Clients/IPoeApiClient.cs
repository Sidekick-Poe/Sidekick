using System.Text.Json;

namespace Sidekick.Apis.Poe.Clients
{
    public interface IPoeApiClient
    {
        JsonSerializerOptions Options { get; }

        Task<TReturn?> Fetch<TReturn>(string path);
    }
}
