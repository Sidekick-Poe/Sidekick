using Sidekick.Common.Platform.EventArgs;
using Sidekick.Common.Settings;

namespace Sidekick.Common.Platform.Input;

/// <summary>
/// Interface for keybind handlers for scroll wheel events.
/// </summary>
public abstract class ScrollWheelHandler : IInputHandler, IDisposable
{
    private readonly ISettingsService settingsService;
    private readonly IKeyboardProvider keyboardProvider;

    protected ScrollWheelHandler(ISettingsService settingsService, IKeyboardProvider keyboardProvider)
    {
        this.settingsService = settingsService;
        this.keyboardProvider = keyboardProvider;
        settingsService.OnSettingsChanged += OnSettingsChanged;
        keyboardProvider.OnScrollDown += ScrollDownHandler;
        keyboardProvider.OnScrollUp += ScrollUpHandler;
    }

    private void OnSettingsChanged(string[] keys)
    {
        _ = Task.Run(async () =>
        {
            Enabled = await GetEnabled();
        });
    }

    /// <summary>
    /// Represents a property that determines whether the scroll wheel handler is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <inheritdoc />
    public int Priority => 0;

    /// <summary>
    /// Determines whether the scroll wheel handler should be enabled based on the current settings.
    /// </summary>
    /// <returns>A task that resolves to true if the handler should be enabled; otherwise, false.</returns>
    protected abstract Task<bool> GetEnabled();

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

    /// <inheritdoc />
    public async Task Initialize()
    {
        Enabled = await GetEnabled();
    }

    private void ScrollDownHandler(ScrollEventArgs args)
    {
        if (!Enabled) return;
        if (args.Masks != "Ctrl") return;
        if (!IsValid()) return;

        OnScrollDown();
        args.Suppress = true;
    }

    private void ScrollUpHandler(ScrollEventArgs args)
    {
        if (!Enabled) return;
        if (args.Masks != "Ctrl") return;
        if (!IsValid()) return;

        OnScrollUp();
        args.Suppress = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        settingsService.OnSettingsChanged -= OnSettingsChanged;
        keyboardProvider.OnScrollDown -= ScrollDownHandler;
        keyboardProvider.OnScrollUp -= ScrollUpHandler;
    }
}
