using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Input;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.General.Keybinds;

public class FindItemKeybindHandler(
    IInputProvider input,
    IClipboardProvider clipboardProvider,
    IProcessProvider processProvider,
    IItemParser itemParser,
    ISettingsService settingsService) : KeybindHandler(settingsService, SettingKeys.KeyFindItems)
{
    private readonly ISettingsService settingsService = settingsService;

    protected override async Task<List<string?>> GetKeybinds() =>
    [
        await settingsService.GetString(SettingKeys.KeyFindItems)
    ];

    public override bool IsValid(string _) => processProvider.IsPathOfExileInFocus;

    public override async Task Execute(string keybind)
    {
        var text = await clipboardProvider.Copy();
        if (text == null)
        {
            await input.PressKey(keybind);
            return;
        }

        var item = itemParser.ParseItem(text);

        string? searchValue = null;
        if (!string.IsNullOrEmpty(item.Definition.UniqueItem?.Name)) searchValue = item.Definition.UniqueItem.Name;
        else if (!string.IsNullOrEmpty(item.Definition.BaseItem?.Name)) searchValue = item.Definition.BaseItem.Name;
        if (string.IsNullOrEmpty(searchValue)) return;

        await clipboardProvider.SetText(searchValue);
        input.ReleaseAltModifier();
        await input.PressKey(
            "Ctrl+F",
            "Ctrl+A",
            "Ctrl+V",
            "Enter");
    }
}
