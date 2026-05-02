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
    /// Gets a value indicating whether the view supports the always on top functionality.
    /// </summary>
    bool SupportsAlwaysOnTop { get; }

    /// <summary>
    /// Opens a view and navigates to the given URL.
    /// </summary>
    /// <param name="url">The URL to navigate to within the view.</param>
    /// <param name="alwaysOnTop">Indicates if the view should always be on top of other windows.</param>
    void Open(string url, bool alwaysOnTop);

    /// <summary>
    /// Closes a view of the specified type.
    /// </summary>
    void Close();

    /// <summary>
    /// Check if an overlay is opened
    /// </summary>
    /// <returns>true if a view is opened. false otherwise</returns>
    bool IsOverlayOpened();
}
