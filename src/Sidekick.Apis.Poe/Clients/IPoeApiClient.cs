namespace Sidekick.Apis.Poe.Clients;

public interface IPoeApiClient
{
    Task<TReturn?> Fetch<TReturn>(string path);
}
