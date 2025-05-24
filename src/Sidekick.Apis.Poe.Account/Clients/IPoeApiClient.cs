namespace Sidekick.Apis.Poe.Account.Clients;

public interface IPoeApiClient
{
    Task<TReturn?> Fetch<TReturn>(string path);
}
