namespace Sidekick.Apis.Poe.Clients.Models;

public class FetchResult<T>
{
    public List<T> Result { get; init; } = new();
}
