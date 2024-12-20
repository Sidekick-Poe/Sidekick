namespace Sidekick.Common.Exceptions;

public class ApiErrorException : SidekickException
{
    public ApiErrorException()
        : base("Sidekick failed to communicate with the API.", "If the official trade website is down, Sidekick will not work.", "Make sure your league is set correctly in the settings.", "Try resetting the cache in the settings.", "Please try again later or open a ticket on GitHub.")
    {
    }
}
