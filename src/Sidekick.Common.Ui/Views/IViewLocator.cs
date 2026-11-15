namespace Sidekick.Common.Ui.Views;

/// <summary>
/// Interface to manage views
/// </summary>
public interface IViewLocator
{
    /// <summary>
    /// The last opened view type.
    /// </summary>
    SidekickViewType LastOpenedType { get; }

    /// <summary>
    /// Check if a view is opened
    /// </summary>
    /// <returns>true if a view is opened. false otherwise</returns>
    bool IsOpened(SidekickViewType type);

    /// <summary>
    /// Opens a view of the specified type and navigates to the given URL.
    /// </summary>
    /// <param name="type">The type of view to open.</param>
    /// <param name="url">The URL to navigate to within the view.</param>
    void Open(SidekickViewType type, string url);

    /// <summary>
    /// Closes the Sidekick view.
    /// </summary>
    void Close(SidekickViewType type);

    /// <summary>
    /// Requests a size for a view, replacing its default size. The size is before zoom and is limited to the screen.
    /// </summary>
    /// <param name="owner">The page that requests the size. Only this owner can reset it, and closing the view clears it.</param>
    void SetSize(SidekickViewType type, int width, int height, object owner);

    /// <summary>
    /// Restores the default size of a view, when the size was requested by the same owner.
    /// </summary>
    void ResetSize(SidekickViewType type, object owner);
}
