using Sidekick.Apis.Poe;
using Sidekick.Common;
using Sidekick.Common.Keybinds;
using Sidekick.Common.Platform;

namespace Sidekick.Modules.General.Keybinds
{
    public class FindItemKeybindHandler(
        IKeyboardProvider keyboard,
        IClipboardProvider clipboardProvider,
        IProcessProvider processProvider,
        IItemParser itemParser,
        ISettingsService settingsService) : KeybindHandler
    {
        public List<string?> GetKeybinds() =>
        [
            settingsService.GetSettings()
                           .Key_FindItems,
        ];

        public bool IsValid(string _) => processProvider.IsPathOfExileInFocus;

        public async Task Execute(string keybind)
        {
            var text = await clipboardProvider.Copy();
            if (text == null)
            {
                await keyboard.PressKey(keybind);
                return;
            }

            var item = await itemParser.ParseItemAsync(text);
            await clipboardProvider.SetText(item.Header.Name);
            await keyboard.PressKey("Ctrl+F", "Ctrl+A", "Ctrl+V", "Enter");
        }
    }
}
