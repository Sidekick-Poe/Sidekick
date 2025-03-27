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
    /// Gets the options specifying the configuration of the current view.
    /// Includes properties such as title, dimensions, and constraints.
    /// </summary>
    ViewOptions Options { get; }

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
}
