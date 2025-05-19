using Sidekick.Common.Initialization;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.General.Keybinds;

public class MouseWheelHandler
(
    ISettingsService settingsService,
    IKeyboardProvider keyboardProvider
) : IInitializableService
{
    private bool MouseWheelNavigateStash { get; set; }

    private bool MouseWheelNavigateStashReverse { get; set; }

    private async void OnSettingsChanged(string[] keys)
    {
        if (keys.Contains(SettingKeys.MouseWheelNavigateStash))
        {
            MouseWheelNavigateStash = await settingsService.GetBool(SettingKeys.MouseWheelNavigateStash);
        }

        if (keys.Contains(SettingKeys.MouseWheelNavigateStashReverse))
        {
            MouseWheelNavigateStashReverse = await settingsService.GetBool(SettingKeys.MouseWheelNavigateStashReverse);
        }
    }

    public int Priority => 0;

    public async Task Initialize()
    {
        MouseWheelNavigateStash = await settingsService.GetBool(SettingKeys.MouseWheelNavigateStash);
        MouseWheelNavigateStashReverse = await settingsService.GetBool(SettingKeys.MouseWheelNavigateStashReverse);
        settingsService.OnSettingsChanged += OnSettingsChanged;
        keyboardProvider.OnScrollDown += OnScrollDown;
        keyboardProvider.OnScrollUp += OnScrollUp;
    }

    private void OnScrollDown(string keybind)
    {
        if (!MouseWheelNavigateStash) return;
        if (keybind != "Ctrl") return;

        keyboardProvider.PressKey(MouseWheelNavigateStashReverse ? "Right" : "Left");
    }

    private void OnScrollUp(string keybind)
    {
        if (!MouseWheelNavigateStash) return;
        if (keybind != "Ctrl") return;

        keyboardProvider.PressKey(MouseWheelNavigateStashReverse ? "Left" : "Right");
    }
}
