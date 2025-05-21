using Sidekick.Common.Platform.Input;
using Sidekick.Common.Settings;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Modules.General.Keybinds;

public class CloseOverlayKeybindHandler(
    IViewLocator viewLocator,
    ISettingsService settingsService) : KeybindHandler(settingsService, SettingKeys.KeyClose)
{
    private readonly ISettingsService settingsService = settingsService;

    protected override async Task<List<string?>> GetKeybinds() =>
    [
        await settingsService.GetString(SettingKeys.KeyClose)
    ];

    public override bool IsValid(string _) => viewLocator.IsOverlayOpened();

    public override Task Execute(string _)
    {
        viewLocator.Close(SidekickViewType.Overlay);
        return Task.CompletedTask;
    }
}
