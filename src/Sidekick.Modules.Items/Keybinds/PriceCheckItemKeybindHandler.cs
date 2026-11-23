using Sidekick.Common.Cache;
using Sidekick.Common.Extensions;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;
using Sidekick.Common.Settings.Input;
using Sidekick.Common.Ui.Views;
namespace Sidekick.Modules.Items.Keybinds;

public class PriceCheckItemKeybindHandler
(
    IViewLocator viewLocator,
    IClipboardProvider clipboardProvider,
    IProcessProvider processProvider,
    ISettingsService settingsService,
    IScopedCacheProvider scopedCacheProvider,
    IInputProvider input
) : KeybindHandler(settingsService, SettingKeys.KeyOpenPriceCheck)
{
    private readonly ISettingsService settingsService = settingsService;

    protected override async Task<List<string?>> GetKeybinds() =>
    [
        await settingsService.GetStringAsync(SettingKeys.KeyOpenPriceCheck)
    ];

    public override bool IsValid(string _) => processProvider.IsPathOfExileInFocus;

    public override async Task Execute(string keybind)
    {
        var text = await clipboardProvider.Copy();
        if (text == null)
        {
            // Copy failures on Linux usually come from the setup (like KDE's Remote Control permission), so the page explains them.
            // While the Linux key grabs are turned off (see LinuxHotkeyBlocker), the game already received the key there.
            if (OperatingSystem.IsLinux())
            {
                viewLocator.Open(SidekickViewType.Overlay, "/item/copy-failed");
                return;
            }

            await input.PressKey(keybind);
            return;
        }

        scopedCacheProvider.Clear();
        viewLocator.Open(SidekickViewType.Overlay, $"/item/{text.EncodeBase64Url()}");
    }
}
