using Sidekick.Apis.Poe.Items;
namespace Sidekick.Apis.PoeNinja.Clients;

public interface INinjaClient
{
    Task<TResponse?> Fetch<TResponse>(GameType game, string path, Dictionary<string, string?>? parameters = null)
        where TResponse : class;
}
