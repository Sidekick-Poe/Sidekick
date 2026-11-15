namespace Sidekick.Common.Exceptions;

public class UnparsableException(string message, params string[] additionalInformation) : SidekickException(message, additionalInformation)
{
    public override bool IsCritical => false;
}
