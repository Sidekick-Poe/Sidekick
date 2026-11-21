using Sidekick.Common.Settings;
using Sidekick.Common.Settings.Input;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Modules.General.Keybinds;

public class CloseOverlayWithEscHandler : KeybindHandler
{
    private readonly ISettingsService settingsService;
    private readonly IViewLocator viewLocator;

    public CloseOverlayWithEscHandler(
        IViewLocator viewLocator,
        ISettingsService settingsService)
        : base(settingsService, SettingKeys.EscapeClosesOverlays)
    {
        this.viewLocator = viewLocator;
        this.settingsService = settingsService;
    }

    protected override Task<List<string?>> GetKeybinds() => Task.FromResult<List<string?>>(
    [
        "Esc",
    ]);

    public override bool IsValid(string _) => settingsService.GetBool(SettingKeys.EscapeClosesOverlays) && viewLocator.IsOpened(SidekickViewType.Overlay);

    public override Task Execute(string _)
    {
        viewLocator.Close(SidekickViewType.Overlay);
        return Task.CompletedTask;
    }
}
