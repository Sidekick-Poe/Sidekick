namespace Sidekick.Apis.Poe2Scout.Clients;

public interface IScoutClient
{
    Task<TResponse?> Fetch<TResponse>(string path, Dictionary<string, string?>? parameters = null)
        where TResponse : class;
}
