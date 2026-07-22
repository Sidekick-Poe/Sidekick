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
    /// Initiates a dragging action for a view at the specified coordinates.
    /// </summary>
    /// <param name="type">The type of view to move.</param>
    /// <param name="pageX">The X-coordinate on the page where the dragging action starts.</param>
    /// <param name="pageY">The Y-coordinate on the page where the dragging action starts.</param>
    void StartMoving(SidekickViewType type, int pageX, int pageY);

    /// <summary>
    /// Stops dragging the specified view.
    /// </summary>
    /// <param name="type">The type of view to move.</param>
    void StopMoving(SidekickViewType type);
}
