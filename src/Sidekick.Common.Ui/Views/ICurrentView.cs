namespace Sidekick.Common.Ui.Views;

/// <summary>
/// Interface to manage views
/// </summary>
public interface ICurrentView
{
    /// <summary>
    /// Event when any property of the view is updated.
    /// </summary>
    event Action<ICurrentView>? ViewChanged;

    /// <summary>
    /// Gets a unique id associated with this view.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the title of the view.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets the width of the view.
    /// </summary>
    int? Width { get; }

    /// <summary>
    /// Gets the height of the view.
    /// </summary>
    int? Height { get; }

    /// <summary>
    /// Gets the minimum width of the view.
    /// </summary>
    int? MinWidth { get; }

    /// <summary>
    /// Gets the minimum height of the view.
    /// </summary>
    int? MinHeight { get; }

    /// <summary>
    /// Gets the current Url of the view.
    /// </summary>
    string Url { get; }

    /// <summary>
    /// Gets the key of the view.
    /// </summary>
    string? Key { get; }

    /// <summary>
    /// Gets the current sidekick view.
    /// </summary>
    SidekickView? Current { get; }

    /// <summary>
    /// Gets a value indicating whether the current view is initialized.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Sets the current view title.
    /// </summary>
    /// <param name="title"></param>
    void SetTitle(string? title);

    /// <summary>
    /// Sets the dimensions and minimum size constraints for the current view.
    /// </summary>
    /// <param name="width">The desired width of the view. Null if not specified.</param>
    /// <param name="height">The desired height of the view. Null if not specified.</param>
    /// <param name="minWidth">The minimum allowable width of the view. Null if not specified.</param>
    /// <param name="minHeight">The minimum allowable height of the view. Null if not specified.</param>
    void SetSize(int? width = null, int? height = null, int? minWidth = null, int? minHeight = null);

    /// <summary>
    /// Initializes a view that was previously opened.
    /// </summary>
    /// <param name="view">The view to initialize.</param>
    Task Initialize(SidekickView view);

    /// <summary>
    /// Closes the current view.
    /// </summary>
    /// <returns>A task.</returns>
    public Task Close();
}
