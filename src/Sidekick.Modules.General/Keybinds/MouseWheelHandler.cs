using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Input;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.General.Keybinds;

public class MouseWheelHandler
(
    ISettingsService settingsService,
    IKeyboardProvider keyboardProvider,
    IProcessProvider processProvider
) : ScrollWheelHandler(settingsService, keyboardProvider)
{
    private readonly ISettingsService settingsService = settingsService;
    private readonly IKeyboardProvider keyboardProvider = keyboardProvider;

    private bool MouseWheelNavigateStashReverse { get; set; }

    protected override async Task<bool> GetEnabled()
    {
        MouseWheelNavigateStashReverse = await settingsService.GetBool(SettingKeys.MouseWheelNavigateStashReverse);
        return await settingsService.GetBool(SettingKeys.MouseWheelNavigateStash);
    }

    protected override bool IsValid() => processProvider.IsPathOfExileInFocus;

    protected override Task OnScrollUp()
    {
        keyboardProvider.PressKey(MouseWheelNavigateStashReverse ? "Left" : "Right");
        return Task.CompletedTask;
    }

    protected override Task OnScrollDown()
    {
        keyboardProvider.PressKey(MouseWheelNavigateStashReverse ? "Right" : "Left");
        return Task.CompletedTask;
    }
}
