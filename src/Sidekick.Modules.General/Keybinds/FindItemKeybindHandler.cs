using Sidekick.Apis.Poe;
using Sidekick.Common.Keybinds;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.General.Keybinds
{
    public class FindItemKeybindHandler(
        IKeyboardProvider keyboard,
        IClipboardProvider clipboardProvider,
        IProcessProvider processProvider,
        IItemParser itemParser,
        ISettingsService settingsService) : KeybindHandler(settingsService)
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
                await keyboard.PressKey(keybind);
                return;
            }

            var item = await itemParser.ParseItemAsync(text);
            await clipboardProvider.SetText(item.Header.ApiName ?? item.Header.ApiType);
            keyboard.ReleaseAltModifier();
            await keyboard.PressKey(
                "Ctrl+F",
                "Ctrl+A",
                "Ctrl+V",
                "Enter");
        }
    }
}
