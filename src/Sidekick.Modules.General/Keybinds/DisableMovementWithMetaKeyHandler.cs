using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Input;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.General.Keybinds;

public class DisableMovementWithMetaKeyHandler : KeybindHandler
{
    public const string SettingKey = "DisableMovementWithMetaKey";

    private readonly ISettingsService settingsService;
    private readonly IProcessProvider processProvider;

    public DisableMovementWithMetaKeyHandler(
        ISettingsService settingsService,
        IProcessProvider processProvider)
        : base(settingsService, SettingKey)
    {
        this.settingsService = settingsService;
        this.processProvider = processProvider;
        settingsService.OnSettingsChanged += OnSettingsChanged;
        _ = UpdateIsValid();
    }

    private bool DisableMovementWithMetaKey { get; set; }

    private async Task UpdateIsValid()
    {
        DisableMovementWithMetaKey = await settingsService.GetBool(SettingKey);
    }

    private void OnSettingsChanged(string[] keys)
    {
        if (keys.Contains(SettingKey))
        {
            _ = UpdateIsValid();
        }
    }

    protected override Task<List<string?>> GetKeybinds() => Task.FromResult<List<string?>>(
    [
        "Meta+W",
        "Meta+A",
        "Meta+S",
        "Meta+D",
        "Meta+Ctrl+W",
        "Meta+Ctrl+A",
        "Meta+Ctrl+S",
        "Meta+Ctrl+D",
        "Meta+Shift+W",
        "Meta+Shift+A",
        "Meta+Shift+S",
        "Meta+Shift+D",
        "Meta+Ctrl+Shift+W",
        "Meta+Ctrl+Shift+A",
        "Meta+Ctrl+Shift+S",
        "Meta+Ctrl+Shift+D",
    ]);

    public override bool IsValid(string _) => DisableMovementWithMetaKey && (processProvider.IsPathOfExileInFocus || processProvider.IsSidekickInFocus);

    public override Task Execute(string _)
    {
        return Task.CompletedTask;
    }
}
