namespace Sidekick.Common.Ui.Views;

/// <summary>
/// Represents the current view implementation for Sidekick with functionalities
/// to manage and interact with views including initialization, creation, and disposal.
/// </summary>
public class CurrentView : ICurrentView
{
    /// <inheritdoc/>
    public event Action? ViewInitialized;

    /// <inheritdoc/>
    public event Action? ViewMinimized;

    /// <inheritdoc/>
    public event Action? ViewMaximized;

    /// <inheritdoc/>
    public event Action? ViewClosed;

    /// <inheritdoc/>
    public ViewOptions Options { get; private set; } = new();

    /// <inheritdoc/>
    public void Initialize(ViewOptions options)
    {
        Options = options;
        ViewInitialized?.Invoke();
    }

    /// <inheritdoc/>
    public void Minimize()
    {
        ViewMinimized?.Invoke();
    }

    /// <inheritdoc/>
    public void Maximize()
    {
        ViewMaximized?.Invoke();
    }

    /// <inheritdoc/>
    public void Close()
    {
        ViewClosed?.Invoke();
    }
}
