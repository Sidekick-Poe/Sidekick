using Sidekick.Common.Platform;
using Sidekick.Common.Platform.EventArgs;
using Sidekick.Common.Platform.Input;
namespace Sidekick.Common.Settings.Input;

/// <summary>
/// Interface for keybind handlers for scroll wheel events.
/// </summary>
public abstract class ScrollWheelHandler : IInputHandler, IDisposable
{
    private readonly IInputProvider inputProvider;

    protected ScrollWheelHandler(IInputProvider inputProvider)
    {
        this.inputProvider = inputProvider;
        inputProvider.OnScrollDown += ScrollDownHandler;
        inputProvider.OnScrollUp += ScrollUpHandler;
    }

    /// <summary>
    /// Determines whether the scroll wheel handler is enabled in the settings. Called on every scroll, so it must not wait.
    /// </summary>
    /// <returns>True if the handler is enabled; otherwise, false.</returns>
    protected abstract bool IsEnabled();

    /// <summary>
    ///     When an event occurs, check if this handler should be executed
    /// </summary>
    /// <returns>True if we need to execute this keybind</returns>
    protected abstract bool IsValid();

    /// <summary>
    /// Handles the scroll-up action triggered by the scroll wheel input.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract Task OnScrollUp();

    /// <summary>
    /// Handles the scroll-down action triggered by the scroll wheel input.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract Task OnScrollDown();

    /// <summary>
    /// Gets a value indicating whether this handler uses Ctrl+scroll, the only scroll it reacts to.
    /// </summary>
    public bool IsCtrlScrollEnabled() => IsEnabled();

    /// <inheritdoc />
    public Task Initialize() => Task.CompletedTask;

    private void ScrollDownHandler(ScrollEventArgs args)
    {
        if (!IsEnabled()) return;
        if (args.Masks != "Ctrl") return;
        if (!IsValid()) return;

        OnScrollDown();
        args.Suppress = true;
    }

    private void ScrollUpHandler(ScrollEventArgs args)
    {
        if (!IsEnabled()) return;
        if (args.Masks != "Ctrl") return;
        if (!IsValid()) return;

        OnScrollUp();
        args.Suppress = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        inputProvider.OnScrollDown -= ScrollDownHandler;
        inputProvider.OnScrollUp -= ScrollUpHandler;
    }
}
