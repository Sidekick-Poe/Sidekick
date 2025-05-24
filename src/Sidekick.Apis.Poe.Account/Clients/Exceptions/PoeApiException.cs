namespace Sidekick.Apis.Poe.Account.Clients.Exceptions;

[Serializable]
public class PoeApiException : Exception
{
    public PoeApiException()
    {
    }

    public PoeApiException(string? message) : base(message)
    {
    }

    public PoeApiException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
