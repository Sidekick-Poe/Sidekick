namespace Sidekick.Common.Ui.Views;

/// <summary>
/// Interface to manage views
/// </summary>
public interface ICurrentView
{
    /// <summary>
    /// An event that is triggered when a view is initialized with specific options.
    /// </summary>
    event Action? ViewInitialized;

    /// <summary>
    /// An event that is triggered when a view is minimized.
    /// </summary>
    event Action? ViewMinimized;

    /// <summary>
    /// An event that is triggered when a view is maximized.
    /// </summary>
    event Action? ViewMaximized;

    /// <summary>
    /// An event that is triggered when a view is closed.
    /// </summary>
    event Action? ViewClosed;

    /// <summary>
    /// An event that is triggered when the view starts being dragged.
    /// </summary>
    event Action<int, int>? ViewStartDragging;

    /// <summary>
    /// An event that is triggered when the view stops being dragged.
    /// </summary>
    event Action? ViewStopDragging;

    /// <summary>
    /// Gets the options specifying the configuration of the current view.
    /// Includes properties such as title, dimensions, and constraints.
    /// </summary>
    ViewOptions Options { get; }

    /// <summary>
    /// Gets a value indicating whether the view is currently being dragged.
    /// </summary>
    bool IsDragging { get; }

    /// <summary>
    /// Initializes the current view with the specified options.
    /// </summary>
    /// <param name="options">The options used to initialize the view.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    void Initialize(ViewOptions options);

    /// <summary>
    /// Minimizes the view.
    /// </summary>
    void Minimize();

    /// <summary>
    /// Maximizes the view.
    /// </summary>
    void Maximize();

    /// <summary>
    /// Closes the view.
    /// </summary>
    void Close();

    /// <summary>
    /// Starts dragging the view with the specified offsets.
    /// The offsets represent the distance from the top-left corner of the view to the point where the drag started.
    /// </summary>
    void StartDragging(int offsetX, int offsetY);

    /// <summary>
    /// Stops dragging the view.
    /// </summary>
    void StopDragging();
}
