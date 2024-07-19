namespace Sidekick.Common.Exceptions;

/// <summary>
///     Represents a sidekick exception.
/// </summary>
public class SidekickException : Exception
{
    public SidekickException(string message)
        : base(message)
    {
    }

    public SidekickException(
        string message,
        string? additionalInformation)
        : base(message)
    {
        AdditionalInformation = additionalInformation;
    }

    public string? AdditionalInformation { get; }
}
