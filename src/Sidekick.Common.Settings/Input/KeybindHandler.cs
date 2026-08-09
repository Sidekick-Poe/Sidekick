using Sidekick.Common.Settings;

namespace Sidekick.Common.Platform.Input;

/// <summary>
///     Interface for keybind handlers
/// </summary>
public abstract class KeybindHandler : IInputHandler, IDisposable
{
    private readonly ISettingsService settingsService;
    protected readonly string SettingKey;

    protected KeybindHandler(ISettingsService settingsService, string settingKey)
    {
        this.settingsService = settingsService;
        SettingKey = settingKey;
        settingsService.OnSettingsChanged += OnSettingsChanged;
    }

    private void OnSettingsChanged(string[] keys)
    {
        if (!keys.Contains(SettingKey))
        {
            return;
        }

        _ = Task.Run(async () =>
        {
            Keybinds = await GetKeybinds();
        });
    }

    /// <summary>
    /// Gets the keybinds that this handler handles.
    /// </summary>
    /// <returns>The list of keybinds.</returns>
    public List<string?> Keybinds { get; private set; } =
    [
    ];

    /// <inheritdoc />
    public int Priority => 0;

    /// <inheritdoc />
    public async Task Initialize()
    {
        Keybinds = await GetKeybinds();
    }

    /// <summary>
    /// Gets the keybinds that this handler handles.
    /// </summary>
    /// <returns>The list of keybinds.</returns>
    protected abstract Task<List<string?>> GetKeybinds();

    /// <summary>
    ///     When a keypress occurs, check if this keybind should be executed
    /// </summary>
    /// <param name="keybind">The keybind that was pressed</param>
    /// <returns>True if we need to execute this keybind</returns>
    public abstract bool IsValid(string keybind);

    /// <summary>
    ///     Executes when a valid keybind is detected
    /// </summary>
    /// <param name="keybind">The keybind that was pressed</param>
    /// <returns>A task</returns>
    public abstract Task Execute(string keybind);

    /// <inheritdoc />
    public void Dispose()
    {
        settingsService.OnSettingsChanged -= OnSettingsChanged;
    }
}
