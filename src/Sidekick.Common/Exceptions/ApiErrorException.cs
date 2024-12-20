namespace Sidekick.Common.Exceptions;

public class ApiErrorException : SidekickException
{
    public ApiErrorException()
        : base("Sidekick failed to communicate with the trade API.", "If the official trade website is down, Sidekick will not work.", "Please try again later or open a ticket on github.")
    {
    }

    public ApiErrorException(string additionalInformation)
        : base("Sidekick failed to communicate with the trade API.", "If the official trade website is down, Sidekick will not work.", additionalInformation, "Please try again later or open a ticket on github.")
    {
    }
}
