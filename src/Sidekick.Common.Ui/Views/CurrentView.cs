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

    private int? width;
    public int? Width
    {
        get => width;
        set
        {
            if (width == value) return;
            width = value;
            OptionsChanged?.Invoke();
        }
    }

    private int? height;
    public int? Height
    {
        get => height;
        set
        {
            if (height == value) return;
            height = value;
            OptionsChanged?.Invoke();
        }
    }
    
    private string? title;
    public string? Title
    {
        get => title;
        set
        {
            if (title == value) return;
            title = value;
            OptionsChanged?.Invoke();
        }
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
