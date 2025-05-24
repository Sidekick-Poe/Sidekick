namespace Sidekick.Apis.Poe.Account.Clients;

public interface IAccountApiClient
{
    Task<TReturn?> Fetch<TReturn>(string path);
}
