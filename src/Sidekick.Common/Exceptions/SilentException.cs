namespace Sidekick.Common.Exceptions;

public class SilentException : SidekickException
{
    public SilentException()
        : base("An exception occured.")
    {
    }

    public SilentException(string? additionalInformation)
        : base("An exception occured.", additionalInformation)
    {
    }
}
