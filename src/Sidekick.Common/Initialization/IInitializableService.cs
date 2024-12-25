namespace Sidekick.Common.Initialization;

/// <summary>
///     Interface for a service that needs to be initialized during startup.
/// </summary>
public interface IInitializableService
{
    /// <summary>
    ///     Gets the priority of execution for this service during the initialization process.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    ///     Initializes the service during startup.
    /// </summary>
    /// <returns>A task.</returns>
    public Task Initialize();
}
