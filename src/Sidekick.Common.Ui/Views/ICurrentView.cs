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
    /// Gets the height of the view.
    /// </summary>
    int? Height { get; }

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
    /// Sets the current view height.
    /// </summary>
    /// <param name="height">The height to set for the current view.</param>
    void SetHeight(int height);

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
