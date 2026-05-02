namespace Sidekick.Common.Ui.Views;

/// <summary>
/// Represents the current view implementation for Sidekick with functionalities
/// to manage and interact with views including initialization, creation, and disposal.
/// </summary>
public class CurrentView : ICurrentView
{
    /// <inheritdoc/>
    public event Action? OptionsChanged;

    /// <inheritdoc/>
    public event Action? Minimized;

    /// <inheritdoc/>
    public event Action? Maximized;

    /// <inheritdoc/>
    public event Action? Closed;

    /// <inheritdoc/>
    public event Action<int, int>? DragStarted;

    /// <inheritdoc/>
    public event Action? DragStopped;

    /// <inheritdoc/>
    public event Action? AlwaysOnTopChanged;

    /// <inheritdoc/>
    public int? Width  { get; private set; }

    /// <inheritdoc/>
    public int? Height { get; private set; }

    /// <inheritdoc/>
    public bool AlwaysOnTop { get; private set; }

    private bool IsDragging { get; set; }

    public void UpdateOptions(int? width, int? height)
    {
        Width = width;
        Height = height;
        OptionsChanged?.Invoke();
    }

    /// <inheritdoc/>
    public void Minimize()
    {
        Minimized?.Invoke();
    }

    /// <inheritdoc/>
    public void Maximize()
    {
        Maximized?.Invoke();
    }

    /// <inheritdoc/>
    public void SetAlwaysOnTop(bool value)
    {
        AlwaysOnTop = value;
        AlwaysOnTopChanged?.Invoke();
    }

    /// <inheritdoc/>
    public void Close()
    {
        Closed?.Invoke();
    }

    /// <inheritdoc/>
    public void StartDragging(int offsetX, int offsetY)
    {
        if (IsDragging) return;

        IsDragging = true;
        DragStarted?.Invoke(offsetX, offsetY);
    }

    /// <inheritdoc/>
    public void StopDragging()
    {
        IsDragging = false;
        DragStopped?.Invoke();
    }
}
