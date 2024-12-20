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

    public List<string> AdditionalInformation { get; } = [];

    public virtual bool IsCritical => true;

    /// <summary>
    /// Gets or sets a value indicating whether the application should exit in response to an exception. This changes the button on the error screen from 'Close' to 'Exit Application'.
    /// </summary>
    /// <remarks>
    /// This property is typically used to signal that a critical error has occurred, requiring the application to shut down. It can be
    /// particularly useful for error handling logic to guide application behavior during exceptional scenarios.
    /// </remarks>
    public bool ExitApplication { get; set; }
}
