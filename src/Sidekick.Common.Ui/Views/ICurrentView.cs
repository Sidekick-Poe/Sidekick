namespace Sidekick.Common.Ui.Views;

/// <summary>
/// Interface to manage views
/// </summary>
public interface ICurrentView
{
    public const int DialogWidth = 400;
    public const int DialogHeight = 220;
    
    /// <summary>
    /// An event that is triggered when a view is updated with specific options.
    /// </summary>
    event Action? OptionsChanged;

    /// <summary>
    /// An event that is triggered when a view is minimized.
    /// </summary>
    event Action? Minimized;

    /// <summary>
    /// An event that is triggered when a view is maximized.
    /// </summary>
    event Action? Maximized;

    /// <summary>
    /// An event that is triggered when a view is closed.
    /// </summary>
    event Action? Closed;

    /// <summary>
    /// An event that is triggered when the view starts being dragged.
    /// </summary>
    event Action<int, int>? DragStarted;

    /// <summary>
    /// An event that is triggered when the view stops being dragged.
    /// </summary>
    event Action? DragStopped;

    int? Width { get; }
    
    int? Height { get; }
    
    void UpdateOptions(int? width, int? height);
    
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
