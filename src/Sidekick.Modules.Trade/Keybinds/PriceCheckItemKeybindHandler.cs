using Sidekick.Common.Extensions;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Input;
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
) : KeybindHandler(settingsService, SettingKeys.KeyOpenPriceCheck)
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
        var advancedText = await clipboardProvider.Copy(true);
        if (text == null)
        {
            await keyboard.PressKey(keybind);
            return;
        }

        viewLocator.Open(SidekickViewType.Overlay, $"/trade/{text.EncodeBase64Url()}/{advancedText?.EncodeBase64Url()}");
    }
}
