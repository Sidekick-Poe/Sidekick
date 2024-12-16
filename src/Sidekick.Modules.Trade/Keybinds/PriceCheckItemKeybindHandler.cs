using Sidekick.Common.Extensions;
using Sidekick.Common.Keybinds;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Modules.Trade.Keybinds;

public class PriceCheckItemKeybindHandler
(
    IViewLocator viewLocator,
    IClipboardProvider clipboardProvider,
    IProcessProvider processProvider,
    ISettingsService settingsService,
    IKeyboardProvider keyboard
) : KeybindHandler(settingsService)
{
    private readonly ISettingsService settingsService = settingsService;

    protected override async Task<List<string?>> GetKeybinds() =>
    [
        await settingsService.GetString(SettingKeys.KeyOpenPriceCheck)
    ];

    public override bool IsValid(string _) => processProvider.IsPathOfExileInFocus;

    public override async Task Execute(string keybind)
    {
        var text = await clipboardProvider.Copy();
        if (text == null)
        {
            await keyboard.PressKey(keybind);
            return;
        }

        await viewLocator.CloseAllOverlays();
        await viewLocator.Open($"/trade/{text.EncodeBase64Url()}");
    }
}
