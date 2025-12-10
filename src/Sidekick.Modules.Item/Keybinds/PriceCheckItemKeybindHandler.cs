using Sidekick.Common.Extensions;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Input;
using Sidekick.Common.Settings;
using Sidekick.Common.Ui.Views;
namespace Sidekick.Modules.Item.Keybinds;

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
        if (text == null)
        {
            await keyboard.PressKey(keybind);
            return;
        }

        viewLocator.Open(SidekickViewType.Overlay, $"/item/{text.EncodeBase64Url()}");
    }
}
