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
    void Open(SidekickViewType type, string url);

    /// <summary>
    /// Closes a view of the specified type.
    /// </summary>
    /// <param name="type">The type of view to close.</param>
    void Close(SidekickViewType type);

    /// <summary>
    /// Check if an overlay is opened
    /// </summary>
    /// <returns>true if a view is opened. false otherwise</returns>
    bool IsOverlayOpened();
}
