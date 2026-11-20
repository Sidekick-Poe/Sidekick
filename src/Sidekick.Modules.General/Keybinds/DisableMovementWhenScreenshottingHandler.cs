using Sidekick.Common.Dialogs;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;
using Sidekick.Common.Settings.Input;

namespace Sidekick.Modules.General.Keybinds;

public class DisableMovementWhenScreenshottingHandler : KeybindHandler
{
    public const string Key = "DisableMovementWhenScreenshotting";

    private readonly ISettingsService settingsService;
    private readonly IProcessProvider processProvider;
    private readonly TransparentDialogProvider transparentDialogProvider;
    private readonly IInputProvider inputProvider;

    public DisableMovementWhenScreenshottingHandler(
        ISettingsService settingsService,
        IProcessProvider processProvider,
        TransparentDialogProvider transparentDialogProvider,
        IInputProvider inputProvider)
        : base(settingsService, Key)
    {
        this.settingsService = settingsService;
        this.processProvider = processProvider;
        this.transparentDialogProvider = transparentDialogProvider;
        this.inputProvider = inputProvider;
    }

    protected override Task<List<string?>> GetKeybinds() => Task.FromResult<List<string?>>(
    [
        "Meta+Shift+S",
    ]);

    public override bool IsValid(string _) => settingsService.GetBool(SettingKey) && processProvider.IsPathOfExileInFocus;

    public override async Task Execute(string keyPress)
    {
        await transparentDialogProvider.Open();
        await inputProvider.WiggleMouse();
        await inputProvider.PressKey(keyPress);
    }
}
