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
    /// Maximizes the specified view type to occupy the full available screen space.
    /// </summary>
    /// <param name="type">The type of view to maximize.</param>
    void Maximize(SidekickViewType type);

    /// <summary>
    /// Minimizes the specified view type, reducing its size or visibility while keeping it active in the background.
    /// </summary>
    /// <param name="type">The type of view to minimize.</param>
    void Minimize(SidekickViewType type);

    /// <summary>
    /// Check if a view is opened
    /// </summary>
    /// <returns>true if a view is opened. false otherwise</returns>
    bool IsOpened(SidekickViewType type);
}
