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

    public int? Width  { get; private set; }
    public int? Height { get; private set; }
    public string? Title { get; private set; }

    public void UpdateOptions(int? width, int? height, string? title)
    {
        Width = width;
        Height = height;
        Title = title;
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
    public void Close()
    {
        Closed?.Invoke();
    }

    private bool IsDragging { get; set; }
    
    public void StartDragging(int offsetX, int offsetY)
    {
        if (IsDragging) return;

        IsDragging = true;
        DragStarted?.Invoke(offsetX, offsetY);
    }

    public void StopDragging()
    {
        IsDragging = false;
        DragStopped?.Invoke();
    }
}
