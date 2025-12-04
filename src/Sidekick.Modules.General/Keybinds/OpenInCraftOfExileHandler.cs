using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Browser;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Input;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.General.Keybinds;

/// <summary>
/// Keybind handler for opening items in Craft of Exile.
/// Does seem to support any languages, at least for the bases.
/// Requires Alt information for PoE1.
/// </summary>
public class OpenInCraftOfExileHandler(
    IClipboardProvider clipboardProvider,
    ISettingsService settingsService,
    IProcessProvider processProvider,
    IBrowserProvider browserProvider,
    IKeyboardProvider keyboard) : KeybindHandler(settingsService, SettingKeys.KeyOpenInCraftOfExile)
{
    private readonly ISettingsService settingsService = settingsService;

    protected override async Task<List<string?>> GetKeybinds() =>
    [
        await settingsService.GetString(SettingKeys.KeyOpenInCraftOfExile)
    ];

    public override bool IsValid(string _) => processProvider.IsPathOfExileInFocus;

    public override async Task Execute(string keybind)
    {
        // We get the item's alt text and url encode it.

        var itemText = await clipboardProvider.Copy(withAlt: true);
        if (itemText == null)
        {
            await keyboard.PressKey(keybind);
            return;
        }

        var game = await settingsService.GetGame();
        var gameParam = game == GameType.PathOfExile1 ? "poe1" : "poe2";
        var encodedItemText = Uri.EscapeDataString(itemText);

        var uriBuilder = new UriBuilder("https://craftofexile.com/")
        {
            Query = $"game={gameParam}&eimport={encodedItemText}"
        };

        browserProvider.OpenUri(uriBuilder.Uri);
    }
}
