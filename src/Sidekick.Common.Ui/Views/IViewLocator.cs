namespace Sidekick.Common.Ui.Views;

/// <summary>
/// Interface to manage views
/// </summary>
public interface IViewLocator
{
    /// <summary>
    /// Gets a value indicating whether the view supports the minimize functionality.
    /// </summary>
    bool SupportsMinimize { get; }

    /// <summary>
    /// Gets a value indicating whether the view supports the maximize functionality.
    /// </summary>
    bool SupportsMaximize { get; }

    /// <summary>
    /// Opens a view of the specified type and navigates to the given URL.
    /// </summary>
    /// <param name="type">The type of view to open.</param>
    /// <param name="url">The URL to navigate to within the view.</param>
    /// <returns>A task that represents the asynchronous operation of opening the view.</returns>
    Task Open(SidekickViewType type, string url);

    /// <summary>
    /// Close all views
    /// </summary>
    Task CloseAll();

    /// <summary>
    /// Close all overlays
    /// </summary>
    Task CloseOverlay();

    /// <summary>
    /// Check if an overlay is opened
    /// </summary>
    /// <returns>true if a view is opened. false otherwise</returns>
    bool IsOverlayOpened();
}
