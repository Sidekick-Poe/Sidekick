namespace Sidekick.Common.Exceptions;

public class ApiErrorException : SidekickException
{
    public ApiErrorException()
        : base("An error occured while trying to get the results from the official trade API. The official trade website may be down. Please try again later or open a ticket on github.")
    {
    }

    public ApiErrorException(string? additionalInformation)
        : base("An error occured while trying to get the results from the official trade API. The official trade website may be down. Please try again later or open a ticket on github.", additionalInformation ?? string.Empty)
    {
    }
}
