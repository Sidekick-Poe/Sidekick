namespace Sidekick.Common.Exceptions;

public class UnparsableException : SidekickException
{
    public UnparsableException()
        : base("Unable to parse this item. Make sure you have set your game language correctly in the settings.")
    {
    }

    public UnparsableException(string? additionalInformation)
        : base("Unable to parse this item. Make sure you have set your game language correctly in the settings.", additionalInformation)
    {
    }
}
