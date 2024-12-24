namespace Sidekick.Common.Exceptions;

/// <summary>
/// Represents the possible actions that can be taken in response to an exception.
/// </summary>
[Flags]
public enum ExceptionActions
{
    CloseWindow = 1,
    ExitApplication = 2,
}
