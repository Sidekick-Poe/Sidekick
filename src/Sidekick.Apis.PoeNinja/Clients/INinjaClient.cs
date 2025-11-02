namespace Sidekick.Apis.PoeNinja.Clients;

public interface INinjaClient
{
    Task<TResponse?> Fetch<TResponse>(string path, Dictionary<string, string?>? parameters = null)
        where TResponse : class;
}
