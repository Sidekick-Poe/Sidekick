using Sidekick.Common.Platform;

namespace Sidekick.AvaloniaServer.Services;

/// <summary>
/// Application service for Blazor Server running in Avalonia.
/// Handles server-specific initialization and shutdown.
/// </summary>
public class AvaloniaServerApplicationService : IApplicationService
{
    /// <summary>
    /// Priority for service initialization (9000 ensures early startup).
    /// </summary>
    public int Priority => 9000;

    /// <summary>
    /// Tracks whether the service has been initialized.
    /// </summary>
    public bool HasInitialized { get; set; }

    /// <summary>
    /// Initializes the application service.
    /// </summary>
    public Task Initialize()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Shuts down the application gracefully.
    /// </summary>
    public void Shutdown()
    {
        Environment.Exit(0);
    }
}
