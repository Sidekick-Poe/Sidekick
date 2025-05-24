using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Input;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.RegexHotkeys.Keybinds;

public class RegexHotkeyHandler(
    ISettingsService settingsService,
    IClipboardProvider clipboard,
    IKeyboardProvider keyboard,
    IProcessProvider processProvider) : KeybindHandler(settingsService, SettingKeys.RegexHotkeys)
{
    private readonly ISettingsService settingsService = settingsService;

    protected override async Task<List<string?>> GetKeybinds()
    {
        var regexHotkeys = await settingsService.GetObject<List<RegexHotkey>>(SettingKeys.RegexHotkeys);
        return regexHotkeys?.Select(x => x.Key).ToList() ?? [];
    }

    public override bool IsValid(string keybind) => processProvider.IsPathOfExileInFocus && Keybinds.Any(x => x == keybind);

    public override async Task Execute(string keybind)
    {
        var regexHotkeys = await settingsService.GetObject<List<RegexHotkey>>(SettingKeys.RegexHotkeys);
        var regexHotkey = regexHotkeys?.FirstOrDefault(x => x.Key == keybind);
        if (regexHotkey == null || string.IsNullOrWhiteSpace(regexHotkey.Regex))
        {
            return;
        }

        string? clipboardValue = null;
        var retainClipboard = await settingsService.GetBool(SettingKeys.RetainClipboard);
        if (retainClipboard)
        {
            clipboardValue = await clipboard.GetText();
        }

        await clipboard.SetText(regexHotkey.Regex);

        await keyboard.PressKey("Ctrl+F", "Ctrl+V");

        if (retainClipboard)
        {
            await Task.Delay(100);
            await clipboard.SetText(clipboardValue);
        }
    }
}
