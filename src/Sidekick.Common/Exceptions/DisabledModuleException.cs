namespace Sidekick.Common.Exceptions;

public class DisabledModuleException : SidekickException
{
    public DisabledModuleException()
        : base("This module is currently disabled in your settings.")
    {
    }

    public DisabledModuleException(string? additionalInformation)
        : base("This module is currently disabled in your settings.", additionalInformation ?? string.Empty)
    {
    }
}
