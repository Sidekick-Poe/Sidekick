namespace Sidekick.Common.Initialization;

/// <summary>
///     Determines the order of execution for the initialization services.
/// </summary>
public enum InitializationPriority
{
    /// <summary>
    ///     Represents a critical priority, will run before any other priorities.
    /// </summary>
    Critical = 0,

    /// <summary>
    ///     Represents a medium priority, will run after critical priority services.
    /// </summary>
    High = 1,

    /// <summary>
    ///     Represents a medium priority, will run after high priority services.
    /// </summary>
    Medium = 2,

    /// <summary>
    ///     Represents a low priority, will run after high and medium priority services.
    /// </summary>
    Low = 3,
}
