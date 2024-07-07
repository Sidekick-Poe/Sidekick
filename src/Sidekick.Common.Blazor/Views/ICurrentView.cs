namespace Sidekick.Common.Blazor.Views;

/// <summary>
/// Interface to manage views
/// </summary>
public interface ICurrentView
{
    /// <summary>
    /// Gets the current sidekick view.
    /// </summary>
    SidekickView? Current { get; }

    /// <summary>
    /// Sets the current sidekick view.
    /// </summary>
    /// <param name="view">The current view.</param>
    void SetCurrent(SidekickView view);

    /// <summary>
    /// Closes the current view.
    /// </summary>
    /// <returns>A task.</returns>
    public Task Close();
}
