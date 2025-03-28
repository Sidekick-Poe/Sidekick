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
        params string[] additionalInformation)
        : base(message)
    {
        AdditionalInformation = additionalInformation.ToList();
    }

    public List<string> AdditionalInformation { get; init; } = [];

    public virtual bool IsCritical { get; init; } = true;

    /// <summary>
    /// Gets or sets the actions that can be taken in response to the exception.
    /// This property represents a combination of one or more values from the <see cref="ExceptionActions"/> enumeration.
    /// </summary>
    public ExceptionActions Actions { get; set; }
}
