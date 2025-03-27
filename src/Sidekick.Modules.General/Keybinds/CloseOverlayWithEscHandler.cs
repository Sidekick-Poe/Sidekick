using Sidekick.Common.Keybinds;
using Sidekick.Common.Settings;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Modules.General.Keybinds;

public class CloseOverlayWithEscHandler : KeybindHandler
{
    private readonly ISettingsService settingsService;
    private readonly IViewLocator viewLocator;

    public CloseOverlayWithEscHandler(
        IViewLocator viewLocator,
        ISettingsService settingsService)
        : base(settingsService)
    {
        this.viewLocator = viewLocator;
        this.settingsService = settingsService;
        settingsService.OnSettingsChanged += OnSettingsChanged;
        _ = UpdateIsValid();
    }

    private bool EscapeClosesOverlays { get; set; }

    private async Task UpdateIsValid()
    {
        EscapeClosesOverlays = await settingsService.GetBool(SettingKeys.EscapeClosesOverlays);
    }

    private void OnSettingsChanged()
    {
        _ = UpdateIsValid();
    }

    protected override Task<List<string?>> GetKeybinds() => Task.FromResult<List<string?>>(
    [
        "Esc",
    ]);

    public override bool IsValid(string _) => EscapeClosesOverlays && viewLocator.IsOverlayOpened();

    public override Task Execute(string _)
    {
        viewLocator.Close(SidekickViewType.Overlay);
        return Task.CompletedTask;
    }
}
